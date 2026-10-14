Option Explicit

' بطاقة الموظف: تُفتح بنقرة مزدوجة على صفّه في شيت البيانات.
' فيها صورته، وبياناته، وأزرار بريد وواتساب تعمل، وسجلّ عملياته.
' والنقر على أي سطر في السجلّ ينقل المؤشّر إلى مكانه في الشيت والبطاقة تبقى مفتوحة،
' ولذلك تُعرض بالوضع غير المقيّد (Modeless): المقيّد يجمّد إكسل خلفه فلا تستطيع رؤية الشيت.

Private mId As String
Private mRow As Long
Private mMail As String
Private mPhone As String

Public Sub LoadEmployee(ByVal id As String)
    Dim ws As Worksheet, f As Range
    mId = id
    Set ws = ThisWorkbook.Worksheets(SHEET_DATA)
    Set f = ws.Range("A:A").Find(id, , , xlWhole)
    If f Is Nothing Then
        MsgBox "لم نجد هذا الرقم الوظيفي", vbExclamation
        Exit Sub
    End If
    mRow = f.Row
    lblName.Caption = f.Offset(0, 1).Value
    lblDept.Caption = "قسم " & f.Offset(0, 2).Value
    lblIdVal.Caption = id
    txtSalary.Text = Format$(f.Offset(0, 3).Value, "#,##0")
    lblDateVal.Caption = Format$(f.Offset(0, 4).Value, "yyyy-mm-dd")
    mMail = CStr(f.Offset(0, 5).Value)
    mPhone = CStr(f.Offset(0, 6).Value)
    txtMail.Text = mMail
    txtPhone.Text = mPhone
    ShowPhoto id
    FillLog id
End Sub

Private Sub ShowPhoto(ByVal id As String)
    ' الصور في مجلّد photos بجانب الملف، باسم الرقم الوظيفي.
    ' مطبّان: LoadPicture لا يقرأ PNG (يردّ «صورة غير صالحة»)، فنستعمل JPG؛
    ' وإن لم توجد صورة لا نترك فراغاً، بل نُظهر أول حرفين من الاسم.
    Dim p As String
    p = ThisWorkbook.Path & Application.PathSeparator & "photos" & Application.PathSeparator & id & ".jpg"
    imgPhoto.Picture = LoadPicture("")
    If Len(Dir$(p)) > 0 Then
        On Error Resume Next
        imgPhoto.Picture = LoadPicture(p)
        imgPhoto.PictureSizeMode = fmPictureSizeModeZoom
        lblInitials.Visible = (Err.Number <> 0)
    Else
        lblInitials.Visible = True
    End If
    If lblInitials.Visible Then lblInitials.Caption = Left$(lblName.Caption, 1)
End Sub

Private Sub FillLog(ByVal id As String)
    ' سجلّ هذا الموظف وحده. العمود الثالث مخفيّ ويحمل رقم الصفّ في الشيت،
    ' وهو ما يجعل القفز إلى السطر ممكناً بعد النقر عليه.
    Dim ws As Worksheet, r As Long, last As Long, n As Long
    lstLog.Clear
    Set ws = ThisWorkbook.Worksheets(SHEET_LOG)
    last = ws.Cells(ws.Rows.Count, 1).End(xlUp).Row
    For r = 2 To last
        If CStr(ws.Cells(r, 1).Value) = id Then
            lstLog.AddItem Format$(ws.Cells(r, 5).Value, "yyyy-mm-dd hh:mm")
            lstLog.List(n, 1) = CStr(ws.Cells(r, 4).Value)
            lstLog.List(n, 2) = CStr(r)
            n = n + 1
        End If
    Next r
    lblLogCount.Caption = n & " عملية"
End Sub

Private Sub lstLog_Click()
    Dim r As Long
    If lstLog.ListIndex < 0 Then Exit Sub
    r = CLng(lstLog.List(lstLog.ListIndex, 2))
    With ThisWorkbook.Worksheets(SHEET_LOG)
        .Activate
        .Cells(r, 1).Resize(1, 5).Select
    End With
End Sub

Private Sub btnSheet_Click()
    ' إلى صفّ الموظف نفسه في شيت البيانات
    With ThisWorkbook.Worksheets(SHEET_DATA)
        .Activate
        .Cells(mRow, 1).Resize(1, 7).Select
    End With
End Sub

Private Sub btnMail_Click()
    If Trim$(mMail) = "" Then
        MsgBox "لا يوجد بريد لهذا الموظف", vbExclamation
        Exit Sub
    End If
    On Error Resume Next
    ThisWorkbook.FollowHyperlink "mailto:" & mMail
End Sub

Private Sub btnWa_Click()
    Dim n As String
    n = OnlyDigits(mPhone)
    If n = "" Then
        MsgBox "لا يوجد رقم لهذا الموظف", vbExclamation
        Exit Sub
    End If
    ' واتساب يريد الرقم بلا صفر ولا رموز، بصيغة الدولة ثم الرقم
    On Error Resume Next
    ThisWorkbook.FollowHyperlink "https://wa.me/" & n
End Sub

Private Function OnlyDigits(ByVal s As String) As String
    Dim i As Long, c As String
    For i = 1 To Len(s)
        c = Mid$(s, i, 1)
        If c >= "0" And c <= "9" Then OnlyDigits = OnlyDigits & c
    Next i
End Function

Private Sub btnSave_Click()
    ' التعديل من البطاقة نفسها: نكتب في صفّ الموظف الذي فتحناه، لا نبحث عنه من جديد
    Dim ws As Worksheet
    If Not IsNumeric(txtSalary.Text) Then
        MsgBox "الراتب يجب أن يكون رقماً", vbExclamation
        txtSalary.SetFocus
        Exit Sub
    End If
    Set ws = ThisWorkbook.Worksheets(SHEET_DATA)
    ws.Cells(mRow, 4).Value = CDbl(txtSalary.Text)
    ws.Cells(mRow, 6).Value = Trim$(txtMail.Text)
    ws.Cells(mRow, 7).Value = Trim$(txtPhone.Text)
    mMail = Trim$(txtMail.Text)
    mPhone = Trim$(txtPhone.Text)
    LogAction "تعديل", mId, lblName.Caption, Mid$(lblDept.Caption, 5)
    MsgBox "تم حفظ التعديل", vbInformation
End Sub

Private Sub btnPdf_Click()
    Dim r As String
    r = PrintCard()
    If Left$(r, 3) = "ERR" Then
        MsgBox "تعذّرت الطباعة: " & Mid$(r, 5), vbExclamation
    Else
        MsgBox "حُفظت البطاقة:" & vbCrLf & r, vbInformation
    End If
End Sub

Public Function PrintCard() As String
    ' البطاقة ورقةً: نملأ شيت الطباعة ببيانات هذا الموظف ثم نصدّره PDF.
    ' الفورم نفسه لا يُطبع، والمخرج ملفّ يُرسل بالبريد أو يُحفظ في الأرشيف.
    ' ونفصل العمل عن الرسالة: الدالة تُرجع المسار، والزرّ هو من يتكلّم. وهذا ما يجعلها
    ' قابلة للاختبار بلا نافذة تنتظر ضغطة.
    Dim ws As Worksheet, p As String
    On Error GoTo bad
    Set ws = ThisWorkbook.Worksheets(SHEET_PRINT)
    ws.Range("C3").Value = lblName.Caption
    ws.Range("C4").Value = Mid$(lblDept.Caption, 5)
    ws.Range("C5").Value = mId
    ws.Range("C6").Value = txtSalary.Text
    ws.Range("C7").Value = lblDateVal.Caption
    ws.Range("C8").Value = mMail
    ws.Range("C9").Value = mPhone
    p = ThisWorkbook.Path & Application.PathSeparator & "بطاقة " & mId & ".pdf"
    ws.ExportAsFixedFormat xlTypePDF, p, , , , , , False
    PrintCard = p
    Exit Function
bad:
    PrintCard = "ERR " & Err.Number & ": " & Err.Description
End Function

Private Sub btnClose_Click()
    Unload Me
End Sub
