import os
import sys

log = []

def log_msg(msg):
    log.append(msg)
    print(msg)

try:
    if not os.path.exists('FormTextSpeedReader.cs'):
        log_msg("Error: FormTextSpeedReader.cs not found")
        sys.exit(1)

    with open('FormTextSpeedReader.cs', 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    log_msg(f"Read {len(lines)} lines.")

    ranges = [
        # FileBrowser
        (321, 375),   # PopulateTreeView, PopulateTreeViewAll
        (378, 469),   # ExpandToLastDirectory
        (560, 587),   # ListViewFile_MouseDown, ListViewFile_MouseClick
        (590, 798),   # ListViewFile_SelectedIndexChanged
        (3167, 3180), # UpdateFileSelectionStatus
        (3471, 3587), # DeleteFiles

        # FileIO
        (180, 216),   # SaveCurrentFile (both overloads)
        (219, 267),   # SaveCurrentFileAs
        (3594, 3684), # SelectedTextSaveAsNew
        (3691, 3902), # WholeTextSaveAsNew

        # Search
        (270, 288),   # ShowFindDialog
        (291, 314),   # ShowReplaceDialog

        # WebBrowser
        (842, 861),   # Navigate
        (864, 867),   # webBrowser1_Navigated
        (870, 900),   # WebBrowser1_DocumentCompleted
        (903, 960),   # ApplyWebBrowserDefaultStyle

        # TextActions
        (1019, 1055), # FontSizeAdd
        (1058, 1094), # FontSizeReduce
        (1096, 1112), # ChangeFont
        (1318, 1321), # AutoRemoveCRButton_Click
        (1324, 1433), # AutoRemoveCR
        (1436, 1439), # AutoRemoveCRWithoutDotAndExclamationMarkButton_Click
        (1442, 1591), # AutoRemoveCRWithoutDotAndExclamationMark
        (1594, 1620), # AutoRemoveCRWithDotAndExclamationMark
        (1623, 1626), # RemoveLeadSpace_Click
        (1629, 1654), # RemoveLeadingAndTrailingSpaces
        (1657, 1729), # ProcessRemoveLeadingSpaces
        (1731, 1739), # ProcessLineForLead
        (1740, 1748), # RemoveLeadingFullWhitespace
        (1750, 1817), # ProcessRemoveEndingSpaces
        (1818, 1833), # RemoveTrailingSpacesFull
        (3262, 3370)  # KeepTwoCRBetweenLines
    ]

    lines_to_remove = set()
    for start, end in ranges:
        for i in range(start, end + 1):
            lines_to_remove.add(i)

    # Verification
    check_map = {
        180: "private void SaveCurrentFile()",
        219: "private void SaveCurrentFileAs()",
        270: "private void ShowFindDialog()",
        321: "private void PopulateTreeView(int subFolderDeepthLimited)"
    }
    
    for lineno, content in check_map.items():
        if lineno <= len(lines):
            line_content = lines[lineno-1].strip()
            if content not in line_content:
                log_msg(f"WARNING: Line {lineno} content mismatch. Expected '{content}', found '{line_content}'")
            else:
                log_msg(f"Line {lineno} verification passed.")

    new_lines = []
    for i, line in enumerate(lines, 1):
        if i not in lines_to_remove:
            new_lines.append(line)
            
    with open('FormTextSpeedReader.cs', 'w', encoding='utf-8') as f:
        f.writelines(new_lines)
        
    log_msg(f"Cleanup complete. Wrote {len(new_lines)} lines.")

except Exception as e:
    log_msg(f"Error: {e}")

with open('cleanup_log.txt', 'w', encoding='utf-8') as f:
    f.write('\n'.join(log))

