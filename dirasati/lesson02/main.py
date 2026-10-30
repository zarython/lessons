import datetime

import flet as ft

NAVY = "#0F2A4A"
NAVY_2 = "#16506E"
GOLD = "#C9A227"
GOLD_SOFT = "#F4E7B8"
CREAM = "#F6F3EC"
INK = "#1D2733"
TEAL = "#0E9AA7"
MUTED = "#6B7684"
FIELD = ft.OutlineInputBorder(border_radius=12, side=ft.BorderSide(1, "black"))
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
    "french": ("اللغة الفرنسية", ft.Icons.TRANSLATE, "#5B6CC2"),
    "islamic": ("التربية الإسلامية", ft.Icons.MOSQUE, "#16796F"),
    "history": ("التاريخ", ft.Icons.ACCOUNT_BALANCE, "#7B3F61"),
    "geography": ("الجغرافيا", ft.Icons.PUBLIC, "#2F8FC9"),
    "computer": ("الحاسوب", ft.Icons.COMPUTER, "#1F3A68"),
    "philosophy": ("الفلسفة", ft.Icons.PSYCHOLOGY, "#9C5BC2"),
    "other": ("مادة أخرى", ft.Icons.SCHOOL, "#7A8594"),
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


def parse_day(text):
    try:
        day, month = text.strip().split("/")
        exam = datetime.date(today.year, int(month), int(day))
    except ValueError:
        return None
    if exam < today:
        exam = exam.replace(year=today.year + 1)
    return (exam - today).days


def subject_card(s, on_edit):
    kind, icon, color = KINDS[s["kind"]]
    exam = today + datetime.timedelta(days=s["days"])
    pencil = ft.IconButton(ft.Icons.EDIT, icon_color=MUTED, icon_size=18, tooltip="تعديل", on_click=lambda e: on_edit(s))
    title = ft.Row([T(s["name"], 21, "x"), pencil], spacing=0)
    head = ft.Row([
        icon_disc(icon, color, 54),
        ft.Column([title, pill(LEVELS[s["level"]], color)], spacing=2, tight=True),
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


def add_tile(on_click):
    return ft.Container(
        ft.Container(
            ft.Column([
                icon_disc(ft.Icons.ADD, TEAL, 80),
                T("أضف مادة", 20, "x", TEAL),
                T("اسمها، موعد امتحانها، ودروسها", 14, "s", MUTED),
            ], spacing=8, horizontal_alignment=ft.CrossAxisAlignment.CENTER, alignment=ft.MainAxisAlignment.CENTER),
            height=300,
            border_radius=18,
            border=ft.Border.all(2, ft.Colors.with_opacity(0.4, TEAL)),
            bgcolor=ft.Colors.with_opacity(0.04, TEAL),
            on_click=on_click,
            ink=True,
        ),
        col={"xs": 12, "md": 6, "xl": 4},
        padding=ft.Padding.symmetric(horizontal=8),
    )


def kind_chip(key, on, pick):
    label, icon, color = KINDS[key]
    return ft.Container(
        ft.Row([
            ft.Icon(icon, color="white" if on else color, size=18),
            T(label, 13, "b", "white" if on else INK),
        ], spacing=4, tight=True),
        bgcolor=color if on else ft.Colors.with_opacity(0.08, color),
        border_radius=18,
        padding=ft.Padding.symmetric(horizontal=10, vertical=6),
        on_click=lambda e: pick(key),
        ink=True,
    )


def subject_form(subject, on_save, on_close):
    chosen = {"kind": "math", "level": 2}
    due = ""
    if subject:
        chosen = {"kind": subject["kind"], "level": subject["level"]}
        d = today + datetime.timedelta(days=subject["days"])
        due = f"{d.day}/{d.month}"
    name = ft.TextField(label="اسم المادة", value=subject["name"] if subject else KINDS["math"][0], border=FIELD)
    exam = ft.TextField(label="موعد الامتحان", hint_text="مثلاً: 15/11", border=FIELD, value=due)
    hint = T("", 13, "b", TEAL)
    kinds = ft.Row(wrap=True, spacing=6, run_spacing=6)

    def show_kinds():
        kinds.controls = [kind_chip(k, k == chosen["kind"], pick_kind) for k in KINDS]

    def pick_kind(key):
        if not name.value or name.value == KINDS[chosen["kind"]][0]:
            name.value = KINDS[key][0]
        chosen["kind"] = key
        show_kinds()
        kinds.update()
        name.update()

    show_kinds()

    def on_exam(e):
        left = parse_day(exam.value)
        hint.value = f"{day_title(today + datetime.timedelta(days=left))} · {when(left)}" if left is not None else ""
        hint.update()

    exam.on_change = on_exam
    level = ft.SegmentedButton(
        segments=[ft.Segment(value=str(k), label=T(v, 14, "s")) for k, v in LEVELS.items()],
        selected=[str(chosen["level"])],
        show_selected_icon=False,
        on_change=lambda e: chosen.update(level=int(e.control.selected[0])),
    )

    def save(e):
        left = parse_day(exam.value)
        if left is None:
            hint.value = "اكتب التاريخ هكذا: 15/11"
            hint.update()
            return
        on_save({
            "name": (name.value or "").strip() or KINDS[chosen["kind"]][0],
            "kind": chosen["kind"],
            "days": left,
            "level": chosen["level"],
        })

    return ft.Container(
        ft.Column([
            T("تعديل المادة" if subject else "مادة جديدة", 24, "x", NAVY),
            kinds,
            name,
            ft.Column([exam, hint], spacing=4),
            ft.Row([T("صعوبتها", 15, "b", expand=True), level]),
            ft.Row([
                ft.TextButton(content=T("إلغاء", 15, "s", MUTED), on_click=lambda e: on_close()),
                ft.FilledButton(
                    content=T("حفظ", 15, "b", "white"), icon=ft.Icons.CHECK, on_click=save, style=ft.ButtonStyle(bgcolor=TEAL),
                ),
            ], alignment=ft.MainAxisAlignment.END),
        ], spacing=14, tight=True, horizontal_alignment=ft.CrossAxisAlignment.STRETCH),
        width=760,
        padding=26,
        border_radius=22,
        bgcolor="white",
        shadow=ft.BoxShadow(blur_radius=40, color=ft.Colors.with_opacity(0.3, "black"), offset=ft.Offset(0, 12)),
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
    cards = ft.ResponsiveRow(spacing=0, run_spacing=16)
    body = ft.Container(
        ft.Column([cards], scroll=ft.ScrollMode.AUTO),
        expand=True,
        bgcolor=CREAM,
        padding=ft.Padding.symmetric(horizontal=24, vertical=18),
        image=ft.DecorationImage(src="/ornaments/pattern_faint.png", repeat=ft.ImageRepeat.REPEAT),
    )
    host = ft.Container(visible=False, left=0, top=0, right=0, bottom=0)

    def refresh():
        cards.controls = [subject_card(s, open_form) for s in subjects] + [add_tile(lambda e: open_form(None))]
        page.update()

    def close_form():
        host.visible = False
        host.content = None
        host.update()

    def open_form(subject):
        def save(data):
            if subject:
                subject.update(data)
            else:
                subjects.append(data)
            close_form()
            refresh()

        form = subject_form(subject, save, close_form)
        host.content = ft.Container(
            form, alignment=ft.Alignment.CENTER, bgcolor=ft.Colors.with_opacity(0.45, "black"), expand=True,
        )
        host.visible = True
        host.update()

    page.add(ft.Stack([ft.Column([bar, body], spacing=0, expand=True), host], expand=True))
    refresh()


ft.run(main, assets_dir="assets")
