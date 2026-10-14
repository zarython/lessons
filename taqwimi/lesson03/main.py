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
KINDS = {"موعد": "#2F6FD0", "مصروف": "#D2453A"}

# أسماء الأشهر والأيام
MONTHS = ["يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
          "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"]
WEEKDAYS = ["الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت", "الأحد"]
HEADER = ["سبت", "أحد", "اثنين", "ثلاثاء", "أربعاء", "خميس", "جمعة"]
HIJRI_MONTHS = ["محرم", "صفر", "ربيع الأول", "ربيع الآخر", "جمادى الأولى", "جمادى الآخرة",
                "رجب", "شعبان", "رمضان", "شوال", "ذو القعدة", "ذو الحجة"]

BOX = 46
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


def month_card(year, month, today, on_click):
    color = MONTH_COLORS[month - 1]
    photo = ft.Image(src=f"months/{month:02d}.jpg", width=7 * BOX, height=110, fit=ft.BoxFit.COVER,
                     border_radius=14)
    title = ft.Text(f"{MONTHS[month - 1]} {year}", size=22, weight=ft.FontWeight.BOLD, color=color)
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

    return ft.Container(
        content=ft.Column(rows, spacing=6),
        width=7 * BOX + 32,
        bgcolor="white",
        border_radius=20,
        padding=16,
        shadow=ft.BoxShadow(blur_radius=24, color="#22000000"),
    )


def main(page: ft.Page):
    page.title = "تقويمي"
    page.rtl = True
    page.bgcolor = CREAM
    page.padding = 0
    page.scroll = ft.ScrollMode.AUTO
    page.window.width = 1200
    page.window.height = 860
    today = datetime.date.today()

    def open_day(e):
        day = e.control.data
        key = day.isoformat()

        lines = []
        for item in entries.get(key, []):
            text = item["title"]
            if item["kind"] == "مصروف":
                text = f"{item['title']}  {item['amount']}"
            dot = ft.Container(width=10, height=10, border_radius=5, bgcolor=KINDS[item["kind"]])
            lines.append(ft.Row([dot, ft.Text(text, size=15)]))

        kind = ft.RadioGroup(
            content=ft.Row([ft.Radio(value="موعد", label="موعد"), ft.Radio(value="مصروف", label="مصروف")]),
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

    bar = ft.Container(
        content=ft.Row(
            [ft.Text("تقويمي", size=30, weight=ft.FontWeight.BOLD, color="white"),
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

    def draw():
        grid.controls = []
        for month in range(1, 13):
            grid.controls.append(month_card(today.year, month, today, open_day))
        page.update()

    draw()


ft.run(main, assets_dir="assets")
