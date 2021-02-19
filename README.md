# testDotNetCoreConsole
ForTest (SSH)

### 專案說明: 有缺有機會再補充
1. CoreTestConsoleApp : 測試用 
2. CreateSqlScript : NTP分類議員審查會並生出SqlScript
3. DownloadNTPAttNoConvert : NTP直接撈取共用表格區的資料夾 不經任何處理(直接存本來檔名)
4. DownloadNTPAttach : NTP下載附件 並整成目前入口網形式(資料夾BmId/檔名BaId) 包含SqlScript
5. NTPBulletinClimb : NTP爬公告(從舊資料庫) 生成SqlScript
6. NTPWebClimb : 直接爬現在NTP的議員照片下來 檔名變成UmId並生出SqlScript
