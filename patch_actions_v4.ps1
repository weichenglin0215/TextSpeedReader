$files = @(
    'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.TextActions.cs',
    'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.cs'
)

$utf8Bom = New-Object System.Text.UTF8Encoding($true)

foreach ($filePath in $files) {
    if (-not (Test-Path $filePath)) { continue }
    Write-Host "Patching $filePath ..."
    
    # Read precisely with BOM
    $content = [System.IO.File]::ReadAllText($filePath, $utf8Bom)
    
    # 1. fix SelectionLength bug (global replacements)
    $content = $content -replace 'richTextBoxText\.SelectionLength = richTextBoxText\.SelectionLength - 2;', 'if (!processWholeDocument) richTextBoxText.SelectionLength -= 2;'
    # Match the specific if pattern seen in Viewed File
    $content = $content -replace 'if \(richTextBoxText\.SelectionLength > 0\) richTextBoxText\.SelectionLength--;', 'if (!processWholeDocument) richTextBoxText.SelectionLength--;'

    # 2. Main Undo Fix Pattern
    # Matches 'if (processWholeDocument) { ... }' specifically focusing on the block content
    $pattern = '(?s)(\s+)if \(processWholeDocument\)\s*\{\r?\n(.*?)\r?\n\s+\}'
    
    $content = [Regex]::Replace($content, $pattern, {
            param($m)
            $indent = $m.Groups[1].Value
            $inner = $m.Groups[2].Value
        
            # Skip if already patched
            if ($inner -match 'SuspendDrawing\(\)') { return $m.Value }
        
            # Only patch if it contains the forbidden assignment
            if ($inner -match 'richTextBoxText\.Text\s*=\s*') {
                $nl = "`r`n"
                $innerIndent = $indent + "    "
                $innerContentIndent = $innerIndent + "    "
            
                # Replace text assignment with SelectAll pattern
                # We assume the assignment line needs to maintain inner indentation
                $newInner = [Regex]::Replace($inner, 'richTextBoxText\.Text\s*=\s*', "richTextBoxText.SelectAll();${nl}${innerContentIndent}richTextBoxText.SelectedText = ")
            
                # Construct the replacement
                # Note: We re-add the newlines and exact braces
                $res = "${indent}if (processWholeDocument)${nl}"
                $res += "${indent}{${nl}"
                $res += "${innerIndent}SuspendDrawing();${nl}"
                $res += "${innerIndent}richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;${nl}"
                $res += "${innerIndent}richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;${nl}"
                $res += "${innerIndent}try${nl}"
                $res += "${innerIndent}{${nl}"
                $res += "${newInner}${nl}"
                $res += "${innerIndent}}${nl}"
                $res += "${innerIndent}finally${nl}"
                $res += "${innerIndent}{${nl}"
                $res += "${innerContentIndent}richTextBoxText.TextChanged += RichTextBoxText_TextChanged;${nl}"
                $res += "${innerContentIndent}richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;${nl}"
                $res += "${innerContentIndent}ResumeDrawing();${nl}"
                $res += "${innerContentIndent}UpdateStatusLabel();${nl}"
                $res += "${innerIndent}}${nl}"
                $res += "${indent}}"
                return $res
            }
            return $m.Value
        })
    
    # Write precisely with BOM and preserved line endings
    [System.IO.File]::WriteAllText($filePath, $content, $utf8Bom)
}
Write-Host "Done."
