const assert = require('node:assert/strict');
const fs = require('node:fs');
const http = require('node:http');
const path = require('node:path');
const { chromium } = require(process.env.REMEMBER_PLAYWRIGHT || 'playwright');
const captures = path.resolve(__dirname, '../Build/Review/Web');
fs.mkdirSync(captures, {recursive: true});

const output = path.resolve(process.env.REMEMBER_WEB_ROOT || path.resolve(__dirname, '../Build/GitHubPages/docs'));
const mimeTypes = { '.html': 'text/html; charset=utf-8', '.js': 'application/javascript', '.txt': 'text/plain' };

function startServer() {
  const prefix = '/RememberGame/';
  // Serve from a repository subpath and leave compressed files unencoded, as on Pages.
  const server = http.createServer((request, response) => {
    const url = new URL(request.url, 'http://localhost');
    if (!url.pathname.startsWith(prefix)) {
      response.writeHead(404).end();
      return;
    }
    const file = path.resolve(output, url.pathname.slice(prefix.length) || 'index.html');
    if (!file.startsWith(output + path.sep) || !fs.existsSync(file) || !fs.statSync(file).isFile()) {
      response.writeHead(404).end();
      return;
    }
    response.writeHead(200, { 'Content-Type': mimeTypes[path.extname(file)] || 'application/octet-stream' });
    fs.createReadStream(file).pipe(response);
  });
  return new Promise((resolve, reject) => {
    server.once('error', reject);
    server.listen(0, '127.0.0.1', () => resolve({ server, url: `http://127.0.0.1:${server.address().port}${prefix}` }));
  });
}

async function main() {
  assert(fs.existsSync(path.join(output, '.nojekyll')), 'GitHub Pages marker is missing');
  const { server, url } = process.env.REMEMBER_WEB_URL
    ? { server: null, url: process.env.REMEMBER_WEB_URL } : await startServer();
  let browser;
  try {
    browser = await chromium.launch({
      executablePath: process.env.REMEMBER_BROWSER,
      headless: true,
      proxy: process.env.REMEMBER_PROXY ? {server: process.env.REMEMBER_PROXY} : undefined,
      args: ['--use-angle=swiftshader', '--enable-unsafe-swiftshader'],
    });
    const context = await browser.newContext({ viewport: { width: 1280, height: 820 } });
    const page = await context.newPage();
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    page.on('response', response => {
      if (response.status() >= 400 && response.url().startsWith(url)) errors.push(`${response.status()} ${response.url()}`);
    });
    page.on('dialog', dialog => {
      errors.push(dialog.message());
      void dialog.dismiss();
    });
    page.on('console', message => {
      if (/REMEMBER_|KITCHEN_/.test(message.text())) console.log(message.text());
      if (/^(?:RuntimeError|Uncaught|InvalidOperationException|NullReferenceException)/.test(message.text())) errors.push(message.text());
    });

    const start = async (suffix = '') => {
      await page.goto(url + suffix, { waitUntil: 'domcontentloaded', timeout: 120000 });
      await page.locator('#loading').waitFor({ state: 'hidden', timeout: 120000 });
      // createUnityInstance resolves before Unity's startup splash has finished.
      await page.waitForTimeout(3500);
      assert(await page.locator('#unity-canvas').evaluate(canvas => canvas === document.activeElement), 'Game did not receive keyboard focus');
    };

    await start();
    await page.waitForTimeout(800);
    const canvasBounds = await page.locator('#unity-canvas').boundingBox();
    const menuClip = { x: canvasBounds.x + canvasBounds.width * .28,
      y: canvasBounds.y + canvasBounds.height * .90, width: canvasBounds.width * .44, height: canvasBounds.height * .08 };
    const titleFocus = await page.screenshot({clip: menuClip});
    await page.screenshot({path: path.join(captures, 'Web-Title.png')});
    await page.keyboard.press('d');
    await page.waitForTimeout(150);
    await page.screenshot({path: path.join(captures, 'Web-Title-D.png')});
    assert(!titleFocus.equals(await page.screenshot({clip: menuClip})), 'D did not move title-menu focus');
    await page.keyboard.press('a');
    await page.waitForTimeout(150);
    assert(titleFocus.equals(await page.screenshot({clip: menuClip})), 'A did not restore title-menu focus');
    await page.keyboard.press('d');
    await page.keyboard.press('Enter');
    await page.waitForTimeout(1300);
    await page.keyboard.press('e');
    await page.waitForTimeout(180);
    await page.keyboard.press('Escape');
    await page.waitForTimeout(180);
    const pauseClip = {x: canvasBounds.x + canvasBounds.width * .35,
      y: canvasBounds.y + canvasBounds.height * .34, width: canvasBounds.width * .30, height: canvasBounds.height * .29};
    const pauseFocus = await page.screenshot({clip: pauseClip});
    await page.keyboard.press('s');
    await page.waitForTimeout(150);
    assert(!pauseFocus.equals(await page.screenshot({clip: pauseClip})), 'S did not move pause-menu focus');
    await page.keyboard.press('w');
    await page.waitForTimeout(150);
    assert(pauseFocus.equals(await page.screenshot({clip: pauseClip})), 'W did not restore pause-menu focus');
    await page.screenshot({path: path.join(captures, 'Web-WASD-Pause.png')});
    if (process.env.REMEMBER_WEB_QUICK === '1') {
      assert.deepEqual(errors, [], 'Browser runtime errors');
      console.log('REMEMBER_LIVE_WEB_VERIFIED: public build loaded, WASD works in title and pause menus, Kitchen is playable.');
      return;
    }
    await start();
    await page.screenshot({ path: path.join(captures, 'Web-Title.png') });
    await page.keyboard.press('Enter');
    await page.waitForTimeout(2450);
    for (let i = 0; i < 3; i++) {
      await page.keyboard.press('e');
      await page.waitForTimeout(160);
    }
    await page.keyboard.down('w');
    await page.waitForTimeout(350);
    await page.keyboard.up('w');
    await page.screenshot({ path: path.join(captures, 'Web-Cemetery.png') });

    const notice = regex => page.waitForEvent('console', { predicate: msg => regex.test(msg.text()), timeout: 130000 });
    const completed = notice(/REMEMBER_SMOKE_PASS/);
    const kitchenCompleted = page.waitForEvent('console', {predicate: msg => /KITCHEN_ROUTE_PASS/.test(msg.text()), timeout: 240000});
    await start('?remember-smoke=1');
    await completed;
    await kitchenCompleted;

    const restored = notice(/REMEMBER_WEB_SAVE_RELOADED/);
    await start('?remember-smoke=1');
    await restored;
    assert.deepEqual(errors, [], 'Browser runtime errors');
    console.log('REMEMBER_WEB_VERIFIED: nested Pages URL, WASD menu navigation, full Acts I and II regression, recipe undo, scripted walking and browser save persistence.');
  } finally {
    if (browser) await browser.close();
    server?.close();
  }
}

main().catch(error => {
  console.error(error);
  process.exitCode = 1;
});
