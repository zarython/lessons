import calendar
import datetime
import json
import os

import flet as ft
from hijridate import Gregorian

# الألوان
NAVY = "#0F2A4A"
GOLD = "#C9A227"
CREAM = "#F6F3EC"
RAMADAN = "#2E9D6A"
EID = "#B0457E"
MONTH_COLORS = ["#3E6FA8", "#5A6FB0", "#3F9A73", "#5DA35A", "#8E9A2F", "#C98A1E",
                "#D2662B", "#C7503A", "#B5683A", "#9C5B33", "#6E6A8F", "#2F5F8A"]
KINDS = {"موعد": "#2F6FD0", "مصروف": "#D2453A", "دوام": "#127A45"}

# أسماء الأشهر والأيام
MONTHS = ["يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
          "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"]
WEEKDAYS = ["الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت", "الأحد"]
HEADER = ["سبت", "أحد", "اثنين", "ثلاثاء", "أربعاء", "خميس", "جمعة"]
HIJRI_MONTHS = ["محرم", "صفر", "ربيع الأول", "ربيع الآخر", "جمادى الأولى", "جمادى الآخرة",
                "رجب", "شعبان", "رمضان", "شوال", "ذو القعدة", "ذو الحجة"]

BOX = 46
BIG = 150
DATA_FILE = "taqwimi.json"


def load():
    if not os.path.exists(DATA_FILE):
        return {}
    with open(DATA_FILE, encoding="utf-8") as f:
        return json.load(f)


def save():
    with open(DATA_FILE, "w", encoding="utf-8") as f:
        json.dump(entries, f, ensure_ascii=False, indent=2)


entries = load()


def arabic_date(day):
    name = WEEKDAYS[day.weekday()]
    month = MONTHS[day.month - 1]
    return f"{name} {day.day} {month} {day.year}"


def hijri(day):
    return Gregorian(day.year, day.month, day.day).to_hijri()


def hijri_date(day):
    h = hijri(day)
    return f"{h.day} {HIJRI_MONTHS[h.month - 1]} {h.year}"


def hijri_span(year, month):
    first = hijri(datetime.date(year, month, 1))
    last = hijri(datetime.date(year, month, calendar.monthrange(year, month)[1]))
    return f"{HIJRI_MONTHS[first.month - 1]} - {HIJRI_MONTHS[last.month - 1]}"


def occasion(h):
    if h.month == 9:
        return RAMADAN
    if h.month == 10 and h.day <= 3:
        return EID
    if h.month == 12 and 10 <= h.day <= 13:
        return EID
    return None


def number(text):
    try:
        return float(text)
    except ValueError:
        return None


def month_total(year, month):
    total = 0
    for key, items in entries.items():
        if key.startswith(f"{year}-{month:02d}"):
            for item in items:
                if item["kind"] == "مصروف":
                    total += number(item["amount"]) or 0
    return total


def day_box(day, month, today, on_click):
    if day.month != month:
        return ft.Container(width=BOX, height=BOX + 8)
    h = hijri(day)
    color = GOLD if day == today else occasion(h)
    dots = []
    for item in entries.get(day.isoformat(), [])[:3]:
        dots.append(ft.Container(width=6, height=6, border_radius=3, bgcolor=KINDS[item["kind"]]))
    return ft.Container(
        content=ft.Column(
            [ft.Text(str(day.day), size=16, weight=ft.FontWeight.BOLD, color="white" if color else NAVY),
             ft.Text(str(h.day), size=10, color="white" if color else "grey"),
             ft.Row(dots, spacing=2, alignment=ft.MainAxisAlignment.CENTER)],
            spacing=0, horizontal_alignment=ft.CrossAxisAlignment.CENTER),
        width=BOX,
        height=BOX + 8,
        border_radius=12,
        bgcolor=color,
        alignment=ft.Alignment.CENTER,
        data=day,
        on_click=on_click,
    )


def month_card(year, month, today, on_click, on_month):
    color = MONTH_COLORS[month - 1]
    photo = ft.Image(src=f"months/{month:02d}.jpg", width=7 * BOX, height=110, fit=ft.BoxFit.COVER,
                     border_radius=14)
    title = ft.Container(ft.Text(f"{MONTHS[month - 1]} {year}", size=22, weight=ft.FontWeight.BOLD, color=color),
                         data=month, on_click=on_month)
    rows = [photo, title, ft.Text(hijri_span(year, month), size=13, color="grey")]

    names = []
    for name in HEADER:
        names.append(ft.Container(ft.Text(name, size=11, color=color), width=BOX, alignment=ft.Alignment.CENTER))
    rows.append(ft.Row(names, spacing=0))

    weeks = calendar.Calendar(firstweekday=5).monthdatescalendar(year, month)
    for week in weeks:
        boxes = []
        for day in week:
            boxes.append(day_box(day, month, today, on_click))
        rows.append(ft.Row(boxes, spacing=0))

    total = month_total(year, month)
    if total:
        rows.append(ft.Container(expand=True))
        rows.append(ft.Text(f"مصاريف الشهر: {total:g}", size=15, weight=ft.FontWeight.BOLD, color=KINDS["مصروف"]))

    return ft.Container(
        content=ft.Column(rows, spacing=6),
        width=7 * BOX + 32,
        height=640,
        bgcolor="white",
        border_radius=20,
        padding=16,
        shadow=ft.BoxShadow(blur_radius=24, color="#22000000"),
    )


def big_day(day, month, today, on_click):
    if day.month != month:
        return ft.Container(width=BIG, height=110)
    h = hijri(day)
    color = GOLD if day == today else occasion(h)
    lines = [ft.Row([ft.Text(str(day.day), size=20, weight=ft.FontWeight.BOLD, color=NAVY),
                     ft.Text(str(h.day), size=12, color="grey")],
                    alignment=ft.MainAxisAlignment.SPACE_BETWEEN)]
    for item in entries.get(day.isoformat(), [])[:3]:
        lines.append(ft.Text(item["title"], size=13, color=KINDS[item["kind"]], max_lines=1))
    return ft.Container(
        content=ft.Column(lines, spacing=2),
        width=BIG,
        height=110,
        padding=8,
        border_radius=12,
        bgcolor=CREAM,
        border=ft.Border.all(3, color) if color else None,
        data=day,
        on_click=on_click,
    )


def month_page(year, month, today, on_click, on_back):
    color = MONTH_COLORS[month - 1]
    photo = ft.Image(src=f"months/{month:02d}.jpg", width=7 * BIG + 36, height=160, fit=ft.BoxFit.COVER,
                     border_radius=16)
    back = ft.IconButton(ft.Icons.ARROW_BACK, on_click=on_back)
    title = ft.Text(f"{MONTHS[month - 1]} {year}", size=30, weight=ft.FontWeight.BOLD, color=color)
    rows = [photo, ft.Row([back, title, ft.Text(hijri_span(year, month), size=16, color="grey")])]

    names = []
    for name in HEADER:
        names.append(ft.Container(ft.Text(name, size=14, color=color), width=BIG, alignment=ft.Alignment.CENTER))
    rows.append(ft.Row(names, spacing=6))

    weeks = calendar.Calendar(firstweekday=5).monthdatescalendar(year, month)
    for week in weeks:
        boxes = []
        for day in week:
            boxes.append(big_day(day, month, today, on_click))
        rows.append(ft.Row(boxes, spacing=6))

    total = month_total(year, month)
    if total:
        rows.append(ft.Text(f"مصاريف الشهر: {total:g}", size=18, weight=ft.FontWeight.BOLD, color=KINDS["مصروف"]))

    return ft.Container(content=ft.Column(rows, spacing=8), bgcolor="white", border_radius=20, padding=20)


def understand(text, today):
    day = today
    if "بعد غد" in text:
        day = today + datetime.timedelta(days=2)
        text = text.replace("بعد غد", "")
    elif "غدا" in text:
        day = today + datetime.timedelta(days=1)
        text = text.replace("غدا", "")
    text = text.replace("اليوم", "")

    words = text.split()
    hour = ""
    if "الساعة" in words:
        k = words.index("الساعة")
        hour = words[k + 1]
        del words[k:k + 2]

    amount = ""
    title = []
    for word in words:
        if number(word) is None:
            title.append(word)
        else:
            amount = word

    title = " ".join(title)
    if hour:
        title = f"{title} الساعة {hour}"
    kind = "مصروف" if amount else "موعد"
    return day, {"kind": kind, "title": title, "amount": amount}


def main(page: ft.Page):
    page.title = "تقويمي"
    page.rtl = True
    page.bgcolor = CREAM
    page.padding = 0
    page.scroll = ft.ScrollMode.AUTO
    page.window.width = 1200
    page.window.height = 860
    today = datetime.date.today()
    state = {"month": None}

    def open_day(e):
        day = e.control.data
        key = day.isoformat()

        def edit(e):
            item = e.control.data
            kind.value = item["kind"]
            title.value = item["title"]
            amount.value = item["amount"]
            amount.visible = item["kind"] == "مصروف"
            title.data = item
            page.update()

        def delete(e):
            entries[key].remove(e.control.data)
            if not entries[key]:
                del entries[key]
            save()
            page.pop_dialog()
            draw()

        lines = []
        for item in entries.get(key, []):
            text = item["title"]
            if item["kind"] == "مصروف":
                text = f"{item['title']}  {item['amount']}"
            dot = ft.Container(width=10, height=10, border_radius=5, bgcolor=KINDS[item["kind"]])
            edit_button = ft.IconButton(ft.Icons.EDIT_OUTLINED, data=item, on_click=edit)
            delete_button = ft.IconButton(ft.Icons.DELETE_OUTLINE, data=item, on_click=delete)
            lines.append(ft.Row([dot, ft.Text(text, size=15, expand=True), edit_button, delete_button]))

        kind = ft.RadioGroup(
            content=ft.Row([ft.Radio(value="موعد", label="موعد"), ft.Radio(value="مصروف", label="مصروف"),
                            ft.Radio(value="دوام", label="دوام")]),
            value="موعد")
        title = ft.TextField(label="العنوان", autofocus=True)
        amount = ft.TextField(label="المبلغ", visible=False)

        def kind_changed(e):
            amount.visible = kind.value == "مصروف"
            page.update()

        kind.on_change = kind_changed

        def add(e):
            if not title.value:
                return
            if kind.value == "مصروف" and number(amount.value) is None:
                amount.error = "اكتب رقماً فقط"
                page.update()
                return
            if title.data:
                entries[key].remove(title.data)
            if key not in entries:
                entries[key] = []
            entries[key].append({"kind": kind.value, "title": title.value, "amount": amount.value})
            save()
            page.pop_dialog()
            draw()

        def close(e):
            page.pop_dialog()

        dialog = ft.AlertDialog(
            title=ft.Text(arabic_date(day)),
            content=ft.Column(lines + [kind, title, amount], tight=True, width=360),
            actions=[ft.TextButton("إلغاء", on_click=close), ft.FilledButton("حفظ", on_click=add)],
        )
        page.show_dialog(dialog)

    def quick_add(e):
        if not quick.value:
            return
        day, item = understand(quick.value, today)
        key = day.isoformat()
        if key not in entries:
            entries[key] = []
        entries[key].append(item)
        save()
        quick.value = ""
        page.show_dialog(ft.SnackBar(ft.Text(f"أضيف إلى {arabic_date(day)}")))
        state["month"] = day.month
        draw()

    quick = ft.TextField(hint_text="أضف بسرعة: موعد مع علاء غدا الساعة 5", width=440, bgcolor="white",
                         on_submit=quick_add)

    bar = ft.Container(
        content=ft.Row(
            [ft.Text("تقويمي", size=30, weight=ft.FontWeight.BOLD, color="white"),
             quick,
             ft.Text(f"{arabic_date(today)}  |  {hijri_date(today)}", size=18, color=GOLD)],
            alignment=ft.MainAxisAlignment.SPACE_BETWEEN,
        ),
        bgcolor=NAVY,
        padding=20,
    )
    page.add(bar)

    grid = ft.Row(wrap=True, spacing=24, run_spacing=24,
                  alignment=ft.MainAxisAlignment.CENTER,
                  vertical_alignment=ft.CrossAxisAlignment.START)
    page.add(ft.Container(grid, padding=30))

    def show_month(e):
        state["month"] = e.control.data
        draw()

    def show_year(e):
        state["month"] = None
        draw()

    def draw():
        grid.controls = []
        if state["month"]:
            grid.controls.append(month_page(today.year, state["month"], today, open_day, show_year))
        else:
            for month in range(1, 13):
                grid.controls.append(month_card(today.year, month, today, open_day, show_month))
        page.update()

    draw()


ft.run(main, assets_dir="assets")
