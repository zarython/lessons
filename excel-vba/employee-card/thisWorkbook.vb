
' نقرة مزدوجة على أي صفّ موظف في شيت البيانات تفتح بطاقته.
' كتبناها في ThisWorkbook لا في الشيت نفسه: حدث المصنّف يصل إلى كل الشيتات،
' فلا نبحث عن وحدة الشيت ولا يهمّنا اسمها البرمجي، ويكفي أن نسأل: أي شيت؟
Private Sub Workbook_SheetBeforeDoubleClick(ByVal Sh As Object, ByVal Target As Range, Cancel As Boolean)
    If Sh.Name <> SHEET_DATA Then Exit Sub
    If Target.Row < 2 Then Exit Sub
    If Target.Column > 7 Then Exit Sub
    If Trim$(CStr(Sh.Cells(Target.Row, 1).Value)) = "" Then Exit Sub
    Cancel = True
    ShowCard CStr(Sh.Cells(Target.Row, 1).Value)
End Sub
