const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.join(__dirname, '..', 'game');
const read = name => JSON.parse(fs.readFileSync(path.join(root, 'data', name + '.json'), 'utf8'));
const system = read('System');
const cemetery = read('Map001');
const memory = read('Map003');

function imageSpec(name, width, height, alpha = false) {
    const png = fs.readFileSync(path.join(root, 'img', name + '.png'));
    assert.equal(png.toString('ascii', 12, 16), 'IHDR');
    assert.deepEqual([png.readUInt32BE(16), png.readUInt32BE(20)], [width, height], name);
    if (alpha) assert.equal(png[25], 6, name + ' must have RGBA frames');
}

assert.equal(system.title1Name, 'RM_Title');
for (const pose of ['Run', 'Crouch']) imageSpec('characters/$RM_Grace_' + pose, 144, 192, true);
for (const map of [cemetery, read('Map002'), memory]) {
    imageSpec('parallaxes/' + map.parallaxName, map.width * 48, map.height * 48);
}
imageSpec('pictures/RM_MemoryChapter', 816, 624);
imageSpec('pictures/RM_MusicBoxStorybook', 816, 624);
assert.equal(cemetery.disableDashing, false);
assert.equal(memory.disableDashing, false);
assert.deepEqual(cemetery.events[1].pages[0].list.find(command => command.code === 236).parameters,
    ['rain', 1, 1, false], 'Rain is animated by MV at low intensity');
assert.ok(read('Items').some(item => item && item.name === 'Music box' && item.itypeId === 2));
assert.ok(read('Items').some(item => item && item.name === 'Faded ribbon' && item.itypeId === 2));
assert.ok(cemetery.events.some(event => event && event.note.includes('RMCrouch')));
assert.ok(memory.events.flatMap(event => event ? event.pages : []).flatMap(page => page.list)
    .some(command => command.code === 231 && command.parameters[1] === 'RM_MemoryChapter'));
for (const [folder, names] of Object.entries({
    bgm: ['Title', 'Farewell', 'Flashback', 'Keepsake'],
    bgs: ['Rain', 'Room'],
    se: ['Tap', 'Grass', 'Stone', 'Wood', 'Lid', 'Drip']
})) {
    for (const name of names) {
        const asset = path.join(root, 'audio', folder, 'RM_Story' + name);
        assert.equal(fs.readFileSync(asset + '.ogg').toString('ascii', 0, 4), 'OggS');
        assert.equal(fs.readFileSync(asset + '.m4a').toString('ascii', 4, 8), 'ftyp');
    }
}
console.log('Pass: cover, sprites, story pictures, animated rain, key items, and both audio formats.');
