CHANGES — bst_backend

2025-10-23 — Program.cs robust-body-fix

- Problem: The POST /api/bst/insert handler attempted to read the request body multiple ways which could fail for non-seekable request streams and left the stream consumed. This caused some insert attempts (when using raw body JSON or integer) to be ignored.
- Fix: Enabled request buffering with `req.EnableBuffering()`, read the body as text, and parsed:
  - plain integer body (e.g. "5")
  - JSON number (e.g. 5)
  - JSON object with `value` property (e.g. { "value": 5 })
  The fix resets `req.Body.Position` and leaves streams open where needed.
- Small addition: added `using System.IO;` for stream reading.

Verification steps performed:
1. Built backend project: `dotnet build bst_backend.csproj` (resolved a file-lock by stopping the running dotnet process), build succeeded.
2. Started backend and performed runtime checks:
   - GET /health => OK
   - POST /api/bst/reset => 200 OK
   - POST /api/bst/insert?value=42 => 200 OK
   - GET /api/bst/tree => returned tree with inserted node
3. Ran project helper `scripts/start-all.cmd` to start backend and frontend and executed smoke test inserts; verified `/api/bst/tree` returns expected nodes.

Notes / next steps:
- Consider adding unit tests for the BstService insert/rotation and for the API-body parsing behavior.
- Current tree is in-memory; add persistence if the tree must survive restarts.

Signed-off-by: automated-patch
