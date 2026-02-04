$files = @(
    'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.TextActions.cs',
    'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.cs'
)

foreach ($filePath in $files) {
    if (-not (Test-Path $filePath)) { 
        Write-Host "File not found: $filePath"
        continue 
    }
    
    Write-Host "Patching $filePath ..."
    
    # Read with UTF8 encoding (handles BOM correctly)
    $content = Get-Content $filePath -Raw -Encoding UTF8
    
    # 1. fix SelectionLength bug (only if not whole document)
    $content = $content -replace 'richTextBoxText\.SelectionLength = richTextBoxText\.SelectionLength - 2;', 'if (!processWholeDocument) richTextBoxText.SelectionLength -= 2;'
    $content = $content -replace 'if \(richTextBoxText\.SelectionLength > 0\) richTextBoxText\.SelectionLength--;', 'if (!processWholeDocument) richTextBoxText.SelectionLength--;'

    # 2. Regex for if(processWholeDocument) blocks
    # Match the 'if' statement including its leading whitespace
    $pattern = '(?s)(\s+)if \(processWholeDocument\)\s*\{\s*(.*?)\s*\}'
    
    $content = [Regex]::Replace($content, $pattern, {
            param($m)
            $indent = $m.Groups[1].Value
            $inner = $m.Groups[2].Value
        
            # Skip if already has SuspendDrawing (already patched)
            if ($inner -match 'SuspendDrawing\(\)') {
                return $m.Value
            }

            # Check if it has .Text assignment
            if ($inner -match 'richTextBoxText\.Text\s*=\s*') {
                $innerIndent = $indent + "    "
                $innerContentIndent = $innerIndent + "    "
            
                # Replace assignment with SelectAll pattern
                # Using $innerContentIndent to maintain a clean look
                $newInner = [Regex]::Replace($inner, 'richTextBoxText\.Text\s*=\s*', "richTextBoxText.SelectAll();`r`n$innerContentIndent`richTextBoxText.SelectedText = ")
            
                # Construct the replacement block
                $result = $indent + "if (processWholeDocument)" + "`r`n"
                $result += $indent + "{" + "`r`n"
                $result += $innerIndent + "SuspendDrawing();" + "`r`n"
                $result += $innerIndent + "richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;" + "`r`n"
                $result += $innerIndent + "richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;" + "`r`n"
                $result += $innerIndent + "try" + "`r`n"
                $result += $innerIndent + "{" + "`r`n"
                $result += $innerContentIndent + $newInner.Trim() + "`r`n"
                $result += $innerIndent + "}" + "`r`n"
                $result += $innerIndent + "finally" + "`r`n"
                $result += $innerIndent + "{" + "`r`n"
                $result += $innerContentIndent + "richTextBoxText.TextChanged += RichTextBoxText_TextChanged;" + "`r`n"
                $result += $innerContentIndent + "richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;" + "`r`n"
                $result += $innerContentIndent + "ResumeDrawing();" + "`r`n"
                $result += $innerContentIndent + "UpdateStatusLabel();" + "`r`n"
                $result += $innerIndent + "}" + "`r`n"
                $result += $indent + "}"
            
                return $result
            }
            return $m.Value
        })
    
    # Write back with UTF8 (adds BOM in PS 5.1)
    Set-Content $filePath $content -Encoding UTF8
}
Write-Host "Done."
