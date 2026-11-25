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
  if (!requested.startsWith(prefix) || !file.startsWith(root + path.sep)) {
    res.writeHead(400).end(); return;
  }
  if (fs.existsSync(file) && fs.statSync(file).isDirectory()) file = path.join(file, 'index.html');
  let code = 200;
  if (!fs.existsSync(file)) {file = path.join(root, '404.html'); code = 404;}
  res.writeHead(code, {'Content-Type': 'text/html; charset=utf-8'});
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
    console.log('ARCHIVE_ROUTES_PASS: all eight archived entry URLs preserve query and hash.');
  } finally {await browser.close(); server.close();}
})().catch(error => {console.error(error);server.close();process.exitCode = 1;});
