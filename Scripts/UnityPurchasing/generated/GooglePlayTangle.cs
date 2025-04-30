// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UBhxKAYmIyEqKlXmfEy/EmxtadhyU1W3xlOgqCTEsujqg1KGAHdAd7jITiZSs6CrhkTpPXajqiXQBRklxaLBPIpr4BKPovGpb98h/FzTNCVZDBdMOmAzaIA6mKgFL2VlJncCHx5Vn4Jggl8p5Imc1SlZ09lMQZPgvgyPrL6DiIekCMYIeYOPj4+Ljo0SBjcdcRFGOv0w/pMSW+2bKhLHUpvWX+IKyP3AsYigithf8+YFO55n+K3CDEqhYCeyUlU4DYbvVCgqiQAL90qDMIZhbtaSSibrpDbx217eekQHiu1e2+jUdq1R+fdQ1/a+GzOC3/yQHzi+JcKMNo1IN7P6/6HgtfwMj4GOvgyPhIwMj4+OL5N7I+TqKNMOI+ix87eHj4yNj46P");
        private static int[] order = new int[] { 12,4,13,3,7,13,12,12,10,10,10,11,13,13,14 };
        private static int key = 142;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
