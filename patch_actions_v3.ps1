$files = @(
    'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.TextActions.cs',
    'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.cs'
)

foreach ($filePath in $files) {
    if (-not (Test-Path $filePath)) { continue }
    Write-Host "Patching $filePath ..."
    $content = Get-Content $filePath -Raw -Encoding UTF8
    
    # fix SelectionLength bug
    $content = $content -replace 'richTextBoxText\.SelectionLength = richTextBoxText\.SelectionLength - 2;', 'if (!processWholeDocument) richTextBoxText.SelectionLength -= 2;'
    $content = $content -replace 'if \(richTextBoxText\.SelectionLength > 0\) richTextBoxText\.SelectionLength--;', 'if (!processWholeDocument) richTextBoxText.SelectionLength--;'

    $pattern = '(?s)(\s+)if \(processWholeDocument\)\s*\{\s*(.*?)\s*\}'
    $content = [Regex]::Replace($content, $pattern, {
            param($m)
            $indent = $m.Groups[1].Value
            $inner = $m.Groups[2].Value
        
            if ($inner -match 'SuspendDrawing\(\)') { return $m.Value }
        
            if ($inner -match 'richTextBoxText\.Text\s*=\s*') {
                $innerIndent = $indent + "    "
                $innerContentIndent = $innerIndent + "    "
                $newLine = "`r`n"
            
                # Replace assignment with SelectAll pattern - using single quotes for the replacement part to avoid $ expansion if needed
                $transformedInner = [Regex]::Replace($inner, 'richTextBoxText\.Text\s*=\s*', "richTextBoxText.SelectAll();${newLine}${innerContentIndent}richTextBoxText.SelectedText = ")

                $sb = New-Object System.Text.StringBuilder
                [void]$sb.Append("${indent}if (processWholeDocument)${newLine}")
                [void]$sb.Append("${indent}{${newLine}")
                [void]$sb.Append("${innerIndent}SuspendDrawing();${newLine}")
                [void]$sb.Append("${innerIndent}richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;${newLine}")
                [void]$sb.Append("${innerIndent}richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;${newLine}")
                [void]$sb.Append("${innerIndent}try${newLine}")
                [void]$sb.Append("${innerIndent}{${newLine}")
                [void]$sb.Append("${innerContentIndent}$($transformedInner.Trim())${newLine}")
                [void]$sb.Append("${innerIndent}}${newLine}")
                [void]$sb.Append("${innerIndent}finally${newLine}")
                [void]$sb.Append("${innerIndent}{${newLine}")
                [void]$sb.Append("${innerContentIndent}richTextBoxText.TextChanged += RichTextBoxText_TextChanged;${newLine}")
                [void]$sb.Append("${innerContentIndent}richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;${newLine}")
                [void]$sb.Append("${innerContentIndent}ResumeDrawing();${newLine}")
                [void]$sb.Append("${innerContentIndent}UpdateStatusLabel();${newLine}")
                [void]$sb.Append("${innerIndent}}${newLine}")
                [void]$sb.Append("${indent}}")
            
                return $sb.ToString()
            }
            return $m.Value
        })
    
    Set-Content $filePath $content -Encoding UTF8
}
Write-Host "Done."
