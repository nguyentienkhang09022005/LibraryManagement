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
  --image tukhoa040505/librarymanagement-api:latest `
  --platform managed `
  --region asia-southeast1 `
  --allow-unauthenticated `
  --port 8080 `
  --set-env-vars $envVars
