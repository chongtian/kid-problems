$TARGETDIR = "$env:BUILD_SOURCESDIRECTORY\kp_web_app\kp-ui\src\environments\environment.prod.ts"
$BUILDNUM = "$env:BUILD_BUILDNUMBER"
Write-Host "$TARGETDIR"
Write-Host "$BUILDNUM"

(Get-Content $TARGETDIR).replace('0.1', "$BUILDNUM") | Set-Content $TARGETDIR