const fs = require('node:fs');
const http = require('node:http');
const path = require('node:path');
const assert = require('node:assert/strict');
const {chromium} = require(process.env.REMEMBER_PLAYWRIGHT || '../RememberMeUnity/QA/node_modules/playwright');
const root = path.resolve(__dirname, '..');
const prefix = '/RememberGame/';
const server = http.createServer((req, res) => {
  const requested = new URL(req.url, 'http://localhost').pathname;
  let file = path.resolve(root, requested.slice(prefix.length));
  if (!requested.startsWith(prefix) || (file !== root && !file.startsWith(root + path.sep))) {
    res.writeHead(400).end(); return;
  }
  if (fs.existsSync(file) && fs.statSync(file).isDirectory()) file = path.join(file, 'index.html');
  let code = 200;
  if (!fs.existsSync(file)) {file = path.join(root, '404.html'); code = 404;}
  const types = {'.html': 'text/html; charset=utf-8', '.png': 'image/png', '.js': 'application/javascript'};
  res.writeHead(code, {'Content-Type': types[path.extname(file)] || 'application/octet-stream'});
  fs.createReadStream(file).pipe(res);
});
(async () => {
  await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
  const browser = await chromium.launch({headless: true, executablePath: process.env.REMEMBER_BROWSER});
  try {
    const page = await browser.newPage();
    await page.route('**/Build/**', route => route.abort());
    for (let version = 2; version <= 9; version++) {
      const expected = `${prefix}archive/web/unity-v${version}/?embedded=1#handoff`;
      await page.goto(`http://127.0.0.1:${server.address().port}${prefix}unity-v${version}/?embedded=1#handoff`);
      await page.waitForURL(url => url.pathname + url.search + url.hash === expected);
      assert(fs.existsSync(path.join(root, `archive/web/unity-v${version}/index.html`)));
    }
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    const home = `http://127.0.0.1:${server.address().port}${prefix}`;
    await page.goto(home);
    assert.equal(await page.locator('html').getAttribute('lang'), 'en');
    assert((await page.locator('[data-doc]').evaluateAll(links => links.map(link => link.href)))
      .every(link => link.endsWith('.md') && !link.endsWith('.zh-CN.md')));
    await page.locator('[data-set-lang="zh"]').click();
    assert.equal(await page.locator('html').getAttribute('lang'), 'zh-CN');
    assert((await page.locator('[data-doc]').evaluateAll(links => links.map(link => link.href)))
      .every(link => link.endsWith('.zh-CN.md')));
    await page.reload();
    assert.equal(await page.locator('html').getAttribute('lang'), 'zh-CN');
    await page.goto(home + '?lang=en');
    assert.equal(await page.locator('html').getAttribute('lang'), 'en');
    assert.deepEqual(errors, []);
    console.log('ARCHIVE_ROUTES_PASS: all eight archived entry URLs preserve query and hash.');
    console.log('PORTAL_LANGUAGES_PASS: English default, Chinese links, stored choice and explicit English selection.');
  } finally {await browser.close(); server.close();}
})().catch(error => {console.error(error);server.close();process.exitCode = 1;});
