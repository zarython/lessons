Option Explicit

' أسماء الشيتات في مكان واحد: لو تغيّر اسم شيت، نغيّره هنا فقط.
Public Const SHEET_DATA As String = "البيانات"
Public Const SHEET_LOG As String = "السجل"
Public Const SHEET_PRINT As String = "بطاقة للطباعة"

Public gCard As frmCard

Public Sub ShowCard(ByVal id As String)
    ' بطاقة واحدة في كل مرة: لو كانت مفتوحة نُحدّثها بدل أن نكدّس نسخاً
    If gCard Is Nothing Then Set gCard = New frmCard
    gCard.LoadEmployee id
    gCard.Show vbModeless
End Sub

Public Sub CardFromSelection()
    ' زرّ الشيت: يفتح بطاقة الصفّ الذي يقف عليه المؤشّر
    Dim ws As Worksheet, r As Long
    Set ws = ThisWorkbook.Worksheets(SHEET_DATA)
    If ActiveSheet.Name <> ws.Name Then
        ws.Activate
        MsgBox "اختر صفّ موظف أولاً", vbExclamation
        Exit Sub
    End If
    r = ActiveCell.Row
    If r < 2 Or Trim$(CStr(ws.Cells(r, 1).Value)) = "" Then
        MsgBox "اختر صفّ موظف أولاً", vbExclamation
        Exit Sub
    End If
    ShowCard CStr(ws.Cells(r, 1).Value)
End Sub

Public Sub LogAction(ByVal what As String, ByVal id As String, ByVal nm As String, ByVal dept As String)
    ' السجلّ نفسه الذي بنيناه في الدرس السادس، نكتب فيه من البطاقة أيضاً
    Dim ws As Worksheet, r As Long
    Set ws = ThisWorkbook.Worksheets(SHEET_LOG)
    r = ws.Cells(ws.Rows.Count, 1).End(xlUp).Row + 1
    ws.Cells(r, 1).Value = id
    ws.Cells(r, 2).Value = nm
    ws.Cells(r, 3).Value = dept
    ws.Cells(r, 4).Value = what
    ws.Cells(r, 5).Value = Now
End Sub

Public Sub TestPdf(ByVal id As String)
    ' للاختبار من خارج البرنامج فقط: لا نافذة رسالة تنتظر ضغطة
    Dim r As String, f As Integer
    ShowCard id
    r = gCard.PrintCard()
    f = FreeFile
    Open Environ$("TEMP") & "\card_log.txt" For Append As #f
    Print #f, r
    Close #f
End Sub

Public Sub TestExportOnly()
    ' عزل: تصدير الشيت وحده بلا فورم، لنعرف أين تقف العملية
    Dim ws As Worksheet, p As String, f As Integer, r As String
    On Error GoTo bad
    Set ws = ThisWorkbook.Worksheets(SHEET_PRINT)
    ws.Range("C3").Value = "اختبار"
    p = ThisWorkbook.Path & Application.PathSeparator & "test.pdf"
    ws.ExportAsFixedFormat xlTypePDF, p, , , , , , False
    r = "exported " & p
    GoTo done
bad:
    r = "ERR " & Err.Number & ": " & Err.Description
done:
    f = FreeFile
    Open Environ$("TEMP") & "\card_log.txt" For Append As #f
    Print #f, r
    Close #f
End Sub
