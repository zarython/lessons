import datetime

import flet as ft

NAVY = "#0F2A4A"
NAVY_2 = "#16506E"
GOLD = "#C9A227"
GOLD_SOFT = "#F4E7B8"
CREAM = "#F6F3EC"
INK = "#1D2733"
FONTS = {
    "Cairo": "/fonts/Cairo-Regular.ttf",
    "CairoSemi": "/fonts/Cairo-SemiBold.ttf",
    "CairoBold": "/fonts/Cairo-Bold.ttf",
    "CairoX": "/fonts/Cairo-ExtraBold.ttf",
}
WEIGHT = {"r": "Cairo", "s": "CairoSemi", "b": "CairoBold", "x": "CairoX"}
KINDS = {
    "math": ("الرياضيات", ft.Icons.FUNCTIONS, "#3B6EA8"),
    "physics": ("الفيزياء", ft.Icons.BOLT, "#6A4C93"),
    "chemistry": ("الكيمياء", ft.Icons.SCIENCE, "#0E9AA7"),
    "biology": ("الأحياء", ft.Icons.ECO, "#2E8B57"),
    "arabic": ("اللغة العربية", ft.Icons.MENU_BOOK, "#B5651D"),
    "english": ("اللغة الإنجليزية", ft.Icons.TRANSLATE, "#D45D8C"),
}
LEVELS = {1: "سهلة", 2: "متوسطة", 3: "صعبة"}
WEEKDAYS = ["الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت", "الأحد"]
MONTHS = ["يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
          "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"]

today = datetime.date.today()
subjects = [
    {"name": "الرياضيات", "kind": "math", "days": 8, "level": 3},
    {"name": "الفيزياء", "kind": "physics", "days": 10, "level": 3},
    {"name": "الكيمياء", "kind": "chemistry", "days": 15, "level": 2},
    {"name": "الأحياء", "kind": "biology", "days": 17, "level": 2},
    {"name": "اللغة العربية", "kind": "arabic", "days": 21, "level": 1},
    {"name": "اللغة الإنجليزية", "kind": "english", "days": 23, "level": 2},
]


def T(value, size=14, weight="r", color=INK, **kw):
    return ft.Text(str(value), size=size, font_family=WEIGHT[weight], color=color, **kw)


def icon_disc(icon, color, size=34):
    return ft.Container(
        content=ft.Icon(icon, color=color, size=size * 0.55),
        width=size,
        height=size,
        border_radius=size / 2,
        bgcolor=ft.Colors.with_opacity(0.14, color),
        alignment=ft.Alignment.CENTER,
    )


def pill(text, color, size=12):
    return ft.Container(
        content=T(text, size, "s", color),
        bgcolor=ft.Colors.with_opacity(0.12, color),
        border_radius=20,
        padding=ft.Padding.symmetric(horizontal=10, vertical=3),
    )


def card(content, padding=16, **kw):
    return ft.Container(
        content=content,
        padding=padding,
        border_radius=18,
        bgcolor="white",
        shadow=ft.BoxShadow(blur_radius=18, color=ft.Colors.with_opacity(0.08, "black"), offset=ft.Offset(0, 6)),
        **kw,
    )


def day_title(d):
    return f"{WEEKDAYS[d.weekday()]} {d.day} {MONTHS[d.month - 1]}"


def when(left):
    if left == 0:
        return "اليوم"
    if left == 1:
        return "غداً"
    if left == 2:
        return "بعد يومين"
    if left <= 10:
        return f"بعد {left} أيام"
    return f"بعد {left} يوماً"


def subject_card(s):
    kind, icon, color = KINDS[s["kind"]]
    exam = today + datetime.timedelta(days=s["days"])
    head = ft.Row([
        icon_disc(icon, color, 54),
        ft.Column([T(s["name"], 21, "x"), pill(LEVELS[s["level"]], color)], spacing=2, tight=True),
    ], spacing=12)
    info = ft.Row([
        ft.Icon(ft.Icons.EVENT, color=color, size=18),
        T(f"الامتحان {day_title(exam)} · {when(s['days'])}", 14, "b"),
    ], spacing=6)
    return ft.Container(
        card(
            ft.Column([head, info], spacing=12),
            padding=18,
            border=ft.Border.only(top=ft.BorderSide(5, color)),
        ),
        col={"xs": 12, "md": 6, "xl": 4},
        padding=ft.Padding.symmetric(horizontal=8),
    )


def main(page: ft.Page):
    page.title = "دراستي"
    page.rtl = True
    page.fonts = FONTS
    page.theme = ft.Theme(font_family="Cairo")
    page.padding = 0
    page.horizontal_alignment = ft.CrossAxisAlignment.STRETCH
    page.window.width = 1200
    page.window.height = 860

    brand = ft.Row([
        ft.Image(src="/icon.png", width=50, height=50),
        ft.Column([T("دراستي", 26, "x", "white"), T("خطّة ذكية لمذاكرتك", 12, "s", GOLD_SOFT)], spacing=-2, tight=True),
    ], spacing=12)
    date_pill = ft.Container(
        ft.Row([
            ft.Icon(ft.Icons.EVENT_AVAILABLE, color=GOLD, size=18),
            T(f"{day_title(today)} {today.year}", 14, "b", "white"),
        ], spacing=6, tight=True),
        bgcolor=ft.Colors.with_opacity(0.12, "white"),
        border_radius=22,
        padding=ft.Padding.symmetric(horizontal=14, vertical=8),
    )
    bar = ft.Container(
        ft.Row([brand, date_pill], alignment=ft.MainAxisAlignment.SPACE_BETWEEN),
        height=88,
        padding=ft.Padding.symmetric(horizontal=24),
        gradient=ft.LinearGradient(begin=ft.Alignment.CENTER_RIGHT, end=ft.Alignment.CENTER_LEFT, colors=[NAVY, NAVY_2]),
        image=ft.DecorationImage(src="/ornaments/pattern_gold.png", repeat=ft.ImageRepeat.REPEAT, opacity=0.45),
    )
    cards = ft.ResponsiveRow([subject_card(s) for s in subjects], spacing=0, run_spacing=16)
    body = ft.Container(
        ft.Column([cards], scroll=ft.ScrollMode.AUTO),
        expand=True,
        bgcolor=CREAM,
        padding=ft.Padding.symmetric(horizontal=24, vertical=18),
        image=ft.DecorationImage(src="/ornaments/pattern_faint.png", repeat=ft.ImageRepeat.REPEAT),
    )
    page.add(ft.Column([bar, body], spacing=0, expand=True))


ft.run(main, assets_dir="assets")
