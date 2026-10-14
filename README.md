# أكواد دروس Zarython

الكود الكامل لكل درس في قناة [Zarython على YouTube](https://www.youtube.com/@zarython) — تعلّم البرمجة بالعربي عبر مشاريع حقيقية.

| السلسلة | المجلد | ما تجده فيه |
|---|---|---|
| بايثون: «تقويمي» من الصفر | [`taqwimi/`](taqwimi) | ملف `main.py` كما انتهى في كل درس من الدروس الستة |
| C# وSQLite: برنامج «متجري» | [`matjari/`](matjari) | مشروع Visual Studio كامل عند نهاية كل حلقة، مع ملف «اقرأني» |
| إكسل وVBA: نظام الموظفين | [`excel-vba/`](excel-vba) | ملف xlsm عند نهاية كل درس، وبطاقة الموظف |

## كيف تستفيد منه

- فاتك درس؟ افتح مجلد الدرس الذي قبله وتابع من هناك.
- تريد المقارنة مع كودك؟ كل ملف هو الحالة النهائية لما كُتب على الشاشة في ذلك الدرس.
- طريقة التشغيل داخل مجلد كل سلسلة.

## تشغيل «تقويمي»

```bash
pip install -r taqwimi/requirements.txt
python taqwimi/lesson06/main.py
```

## تشغيل «متجري»

افتح `matjari/ep05/Matjari/Matjari.slnx` في Visual Studio 2026 Community واضغط F5، أو:

```bash
dotnet run --project matjari/ep05/Matjari/Matjari
```

## ملفات إكسل

افتح ملف xlsm في Excel واضغط «تمكين المحتوى» في الشريط الأصفر، ثم Alt+F11 لرؤية الكود.

---

🌐 [zarython.com](https://zarython.com) · ✉️ support@zarython.com

نبني تطبيقات تجعل حياتك أسهل.
