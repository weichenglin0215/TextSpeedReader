$filePath = 'c:\3D_prj\VS_2022_prj\TextSpeedReader\FormTextSpeedReader.TextActions.cs'
$content = Get-Content $filePath -Raw

# Pattern for blocks with SOME event disabling already
$pattern1 = '(?s)SuspendDrawing\(\);\s*richTextBoxText\.SelectionChanged -= RichTextBoxText_SelectionChanged;\s*try\s*\{(.*?)\}\s*finally\s*\{\s*richTextBoxText\.SelectionChanged \+= RichTextBoxText_SelectionChanged;\s*ResumeDrawing\(\);\s*\}'
$replacement1 = 'SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {$1}
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }'

# Pattern for blocks with NO event disabling
$pattern2 = '(?s)if \(processWholeDocument\)\s*\{\s*SuspendDrawing\(\);\s*try\s*\{(.*?)\}\s*finally\s*\{\s*ResumeDrawing\(\);\s*\}\s*\}'
$replacement2 = 'if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {$1}
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }'

$content = [Regex]::Replace($content, $pattern1, $replacement1)
$content = [Regex]::Replace($content, $pattern2, $replacement2)

Set-Content $filePath $content -Encoding UTF8
