const assert = require('node:assert/strict');
const fs = require('node:fs');
const http = require('node:http');
const path = require('node:path');
const { chromium } = require(process.env.REMEMBER_PLAYWRIGHT || 'playwright');

const output = path.resolve(__dirname, '../Build/GitHubPages/docs');
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
  const { server, url } = await startServer();
  let browser;
  try {
    browser = await chromium.launch({
      executablePath: process.env.REMEMBER_BROWSER,
      headless: true,
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
      if (message.text().includes('REMEMBER_')) console.log(message.text());
      if (/^(?:RuntimeError|Uncaught|InvalidOperationException|NullReferenceException)/.test(message.text())) errors.push(message.text());
    });

    const start = async (suffix = '') => {
      await page.goto(url + suffix, { waitUntil: 'domcontentloaded', timeout: 120000 });
      await page.locator('#loading').waitFor({ state: 'hidden', timeout: 120000 });
      assert(await page.locator('#unity-canvas').evaluate(canvas => canvas === document.activeElement), 'Game did not receive keyboard focus');
    };

    await start();
    await page.screenshot({ path: path.join(__dirname, 'Web-Title.png') });
    await page.keyboard.press('Enter');
    await page.waitForTimeout(2450);
    for (let i = 0; i < 3; i++) {
      await page.keyboard.press('e');
      await page.waitForTimeout(160);
    }
    await page.keyboard.down('w');
    await page.waitForTimeout(350);
    await page.keyboard.up('w');
    await page.screenshot({ path: path.join(__dirname, 'Web-Cemetery.png') });

    const notice = regex => page.waitForEvent('console', { predicate: msg => regex.test(msg.text()), timeout: 130000 });
    const completed = notice(/REMEMBER_SMOKE_PASS/);
    await start('?remember-smoke=1');
    await completed;

    const restored = notice(/REMEMBER_WEB_SAVE_RELOADED/);
    await start('?remember-smoke=1');
    await restored;
    assert.deepEqual(errors, [], 'Browser runtime errors');
    console.log('REMEMBER_WEB_VERIFIED: nested Pages URL, keyboard session, full Act I regression and save persistence after browser reload.');
  } finally {
    if (browser) await browser.close();
    server.close();
  }
}

main().catch(error => {
  console.error(error);
  process.exitCode = 1;
});
