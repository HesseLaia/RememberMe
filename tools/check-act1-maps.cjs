const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const game = path.join(__dirname, '..', 'game');
const data = name => JSON.parse(fs.readFileSync(path.join(game, 'data', name + '.json'), 'utf8'));
const system = data('System');
const cemetery = data('Map001');
const flashback = data('Map002');
const tilesets = data('Tilesets');

function dimensions(folder, name, width, height) {
    const file = fs.readFileSync(path.join(game, 'img', folder, name + '.png'));
    assert.equal(file.toString('ascii', 12, 16), 'IHDR', name);
    assert.deepEqual([file.readUInt32BE(16), file.readUInt32BE(20)], [width, height], name);
}

function walkable(map, column, row) {
    if (column < 0 || row < 0 || column >= map.width || row >= map.height) return false;
    const size = map.width * map.height;
    for (let layer = 3; layer >= 0; layer--) {
        const flag = tilesets[map.tilesetId].flags[map.data[layer * size + row * map.width + column]];
        if (!(flag & 16)) return !(flag & 15);
    }
    return false;
}

function reachable(map, start, blocked = []) {
    assert.ok(walkable(map, ...start), 'Start tile is walkable');
    const seen = new Set();
    const queue = [start];
    for (const [column, row] of queue) {
        const key = [column, row].join();
        if (seen.has(key) || blocked.includes(key) || !walkable(map, column, row)) continue;
        seen.add(key);
        queue.push([column + 1, row], [column - 1, row], [column, row + 1], [column, row - 1]);
    }
    return seen;
}

function expectAdjacent(map, reachableTiles, ids) {
    for (const id of ids) {
        const { x, y, name } = map.events[id];
        assert.ok([[x - 1, y], [x + 1, y], [x, y - 1], [x, y + 1]]
            .some(tile => reachableTiles.has(tile.join())), name + ' is approachable');
    }
}

assert.equal(cemetery.parallaxName, '!RM_CemeteryOvercast', 'Rain artwork is wired to the cemetery');
const memory = data('Map003');
assert.equal(memory.parallaxName, '!RM_HomeStorybook');
assert.equal(data('MapInfos')[3].id, 3);
for (const map of [cemetery, flashback, memory]) {
    dimensions('parallaxes', map.parallaxName, map.width * 48, map.height * 48);
    assert.equal(map.width * map.height * 6, map.data.length);
}
dimensions('titles1', system.title1Name, 816, 624);
dimensions('pictures', 'RM_MusicBoxStorybook', 816, 624);
dimensions('system', 'RM_StoryPaper', 192, 192);
dimensions('parallaxes', '!RM_CemeteryCanopyStory', 1152, 864);

const graveside = reachable(cemetery, [system.startX, system.startY], ['13,12', '14,7', '8,9', '17,10', '16,7']);
for (let column = 10; column <= 13; column++) assert.ok(graveside.has([column, 6].join()));
assert.ok(!walkable(cemetery, 11, 5), 'Grave base is solid');
assert.ok(!walkable(cemetery, 5, 12), 'Bench is solid');
expectAdjacent(cemetery, graveside, [2, 3, 4, 5, 6, 9, 13, 16, 17, 18, 19, 20]);
assert.ok(walkable(flashback, 6, 8), 'Open floor is solid ground, not a wall');

const openBox = cemetery.events[21].pages.find(page => page.conditions.switch1Id === 9);
const transfer = openBox.list.find(command => command.code === 201);
assert.deepEqual(transfer.parameters.slice(0, 2), [0, 3]);
assert.ok(openBox.list.some(command => command.code === 231 && command.parameters[1] === 'RM_MusicBoxStorybook'));
const inside = reachable(memory, transfer.parameters.slice(2, 4), ['12,8']);
expectAdjacent(memory, inside, [1, 2, 3, 4, 5, 7]);
assert.ok(memory.events[1].pages[0].list.some(command => command.code === 121 &&
    command.parameters.join() === '5,5,0'), 'Only reunion completes the upgraded chapter');
assert.ok(cemetery.events[10].pages.some(page => page.conditions.switch1Id === 9 && page.trigger === 0));
console.log('Pass: MV image dimensions, collision reachability, and native event chain.');
