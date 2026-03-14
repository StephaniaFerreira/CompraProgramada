# 1. Configurações
$serviceName = "CotacaoB3Worker"
$displayName = "Cotacao B3 Worker Service"
$publishDir = "C:\Servicos\CotacaoB3"
$exeName = "CotacaoWorker.exe"

# 2. Publicar o projeto
Write-Host "--- Publicando projeto ---" -ForegroundColor Cyan

dotnet publish ".\CotacaoWorker\CotacaoWorker.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -o $publishDir


# 3. Verificar se serviço já existe
$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($service) {

    Write-Host "--- Serviço já existe ---" -ForegroundColor Yellow

    if ($service.Status -ne "Stopped") {
        Write-Host "--- Parando serviço ---" -ForegroundColor Yellow
        Stop-Service $serviceName -Force
        Start-Sleep -Seconds 2
    }

    Write-Host "--- Removendo serviço ---" -ForegroundColor Red
    sc.exe delete $serviceName

    Start-Sleep -Seconds 2
}

# 4. Criar serviço novamente
Write-Host "--- Criando serviço ---" -ForegroundColor Green

$binPath = "`"$publishDir\$exeName`""

sc.exe create $serviceName binPath= $binPath start= auto displayName= "$displayName"


# 5. Iniciar serviço
Write-Host "--- Iniciando serviço ---" -ForegroundColor Green
Start-Service $serviceName

Write-Host "--- Deploy concluído ---" -ForegroundColor White
Read-Host -Prompt "Processo finalizado. Pressione Enter para fechar esta janela."
