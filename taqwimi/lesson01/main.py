import calendar
import datetime

import flet as ft

# الألوان
NAVY = "#0F2A4A"
GOLD = "#C9A227"
CREAM = "#F6F3EC"

# أسماء الأشهر والأيام
MONTHS = ["يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
          "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"]
WEEKDAYS = ["الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت", "الأحد"]
HEADER = ["سبت", "أحد", "اثنين", "ثلاثاء", "أربعاء", "خميس", "جمعة"]


def arabic_date(day):
    name = WEEKDAYS[day.weekday()]
    month = MONTHS[day.month - 1]
    return f"{name} {day.day} {month} {day.year}"


def day_box(number, is_today):
    if number == 0:
        return ft.Container(width=64, height=64)
    return ft.Container(
        content=ft.Text(str(number), size=20, weight=ft.FontWeight.BOLD,
                        color="white" if is_today else NAVY),
        width=64,
        height=64,
        border_radius=32,
        bgcolor=GOLD if is_today else None,
        alignment=ft.Alignment.CENTER,
    )


def month_card(year, month, today):
    title = ft.Text(f"{MONTHS[month - 1]} {year}", size=28, weight=ft.FontWeight.BOLD, color=NAVY)
    rows = [title]

    names = []
    for name in HEADER:
        names.append(ft.Container(ft.Text(name, size=14, color="grey"), width=64, alignment=ft.Alignment.CENTER))
    rows.append(ft.Row(names))

    weeks = calendar.Calendar(firstweekday=5).monthdayscalendar(year, month)
    for week in weeks:
        boxes = []
        for number in week:
            is_today = number == today.day and month == today.month and year == today.year
            boxes.append(day_box(number, is_today))
        rows.append(ft.Row(boxes))

    return ft.Container(
        content=ft.Column(rows),
        bgcolor="white",
        border_radius=20,
        padding=24,
        shadow=ft.BoxShadow(blur_radius=24, color="#22000000"),
    )


def main(page: ft.Page):
    page.title = "تقويمي"
    page.rtl = True
    page.bgcolor = CREAM
    page.padding = 0
    page.window.width = 620
    page.window.height = 780
    today = datetime.date.today()

    bar = ft.Container(
        content=ft.Row(
            [ft.Text("تقويمي", size=30, weight=ft.FontWeight.BOLD, color="white"),
             ft.Text(arabic_date(today), size=18, color=GOLD)],
            alignment=ft.MainAxisAlignment.SPACE_BETWEEN,
        ),
        bgcolor=NAVY,
        padding=20,
    )
    page.add(bar)
    page.add(ft.Container(month_card(today.year, today.month, today), padding=30))


ft.run(main)
