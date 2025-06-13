# Đọc các dòng từ .env, bỏ comment và dòng trống
$lines = Get-Content .env | Where-Object { $_ -and ($_ -notmatch '^\s*#') }

# Làm sạch từng dòng
$processedLines = $lines | ForEach-Object {
    $_.Trim() -replace '"', ''
}

# Ghép thành chuỗi set-env-vars
$envVars = $processedLines -join ','

# In kiểm tra
Write-Host "Deploying with env vars: $envVars"

# Deploy bằng gcloud
gcloud run deploy librarymanagement-api `
  --image tukhoa040505/librarymanagement-api:1.0.0 `
  --platform managed `
  --region asia-southeast1 `
  --allow-unauthenticated `
  --port 8080 `
  --set-env-vars $envVars

  
# Giữ lại 3 revision mới nhất, xóa phần còn lại
$oldRevisions = gcloud run revisions list `
  --service=librarymanagement-api `
  --region=asia-southeast1 `
  --sort-by="~metadata.creationTimestamp" `
  --format="value(metadata.name)" | Select-Object -Skip 3

foreach ($rev in $oldRevisions) {
  gcloud run revisions delete $rev --region=asia-southeast1 --quiet
}