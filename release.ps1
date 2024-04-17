if ($PSScriptRoot -match '.+?\\bin\\?') {
    $dir = $PSScriptRoot + "\"
}
else {
    $dir = $PSScriptRoot + "\bin\"
}


foreach($target in ("BepInEx5"))
{
    $copy = $dir + "copy\BepInEx\plugins\ProfileFixer\"

    Remove-Item -Force -Path ($copy) -Recurse -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Force -Path ($copy)

	Copy-Item -Path ($PSScriptRoot + "\ProfileFixer\bin\Debug\ProfileFixer.dll") -Destination ($copy) -Recurse -Force

    Copy-Item -Path ($dir + "\..\README.md") -Destination ($copy) -Recurse -Force
    Copy-Item -Path ($dir + "\..\LICENSE") -Destination ($copy) -Recurse -Force

    $ver = (Get-ChildItem -Path ($PSScriptRoot + "\ProfileFixer\bin\") -Filter ("*ProfileFixer.dll") -Recurse -Force)[0].VersionInfo.FileVersion.ToString() -replace "([\d+\.]+?\d+)[\.0]*$", '${1}'
    Compress-Archive -Path ($dir + "copy\BepInEx") -Force -CompressionLevel "Optimal" -DestinationPath ($dir + "Distance.ProfileFixer" +"_v" + $ver + ".zip")
}

Remove-Item -Force -Path ($dir + "copy") -Recurse -ErrorAction SilentlyContinue