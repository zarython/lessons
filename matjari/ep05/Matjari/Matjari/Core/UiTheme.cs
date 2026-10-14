namespace Matjari.Core;

// ألوان البرنامج وأحجامه هنا: نغيّرها مرّةً فتتغيّر في كلّ شاشة.
public static class UiTheme
{
    // كحليّ الشريط الجانبيّ، وأفتحُ منه لأسفله.
    public static readonly Color Navy = Color.FromArgb(15, 24, 56);
    public static readonly Color NavyLight = Color.FromArgb(22, 32, 74);

    // أزرق الوجهة المفتوحة، وخلفيّة المحتوى، وبياض الشريط العلويّ.
    public static readonly Color Brand = Color.FromArgb(20, 120, 255);
    public static readonly Color Background = Color.FromArgb(245, 248, 255);
    public static readonly Color Surface = Color.White;

    // لون العناوين، ولون الكتابة الهادئة تحتها.
    public static readonly Color Text = Color.FromArgb(13, 23, 48);
    public static readonly Color TextMuted = Color.FromArgb(67, 83, 122);

    // حروف الأزرار الهادئة، وأحمر التحذير لزرّ الخروج.
    public static readonly Color NavText = Color.FromArgb(190, 205, 235);
    public static readonly Color Danger = Color.FromArgb(255, 150, 150);

    // لونا الأزرار الهادئة: خلفيّة باهتة، وأحمرُ يُقرأ على الأبيض.
    public static readonly Color Quiet = Color.FromArgb(232, 238, 250);
    public static readonly Color DeepRed = Color.FromArgb(190, 45, 45);

}
