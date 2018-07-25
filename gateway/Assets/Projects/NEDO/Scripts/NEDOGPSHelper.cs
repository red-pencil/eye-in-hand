using UnityEngine;
using System.Collections;
using System;

/*
┌------┬---------------------------------------------------------------------
｜ 機能 ｜GPS緯度経度データ⇒平面直角座標系変換
├------┼---------------------------------------------------------------------
｜ 説明 ｜<<入力>>
｜      ｜  平面直角座標系番号
｜      ｜    01系:長崎県,鹿児島県のうち北方北緯32度南方北緯27度西方東経128度18分東方東経130度を境界線とする区域内(奄美群島は東経130度13分までを含む)にあるすべての島、小島、環礁及び岩礁
｜      ｜    02系:福岡県,佐賀県,熊本県,大分県,宮崎県,鹿児島県(01系に規定する区域を除く)
｜      ｜    03系:山口県,島根県,広島県
｜      ｜    04系:香川県,愛媛県,徳島県,高知県
｜      ｜    05系:兵庫県,鳥取県,岡山県
｜      ｜    06系:京都府,大阪府,福井県,滋賀県,三重県,奈良県,和歌山県
｜      ｜    07系:石川県,富山県,岐阜県,愛知県
｜      ｜    08系:新潟県,長野県,山梨県,静岡県
｜      ｜    09系:東京都(14系,18系及び19系に規定する区域を除く)福島県,栃木県,茨城県,埼玉県,千葉県,群馬県,神奈川県
｜      ｜    10系:青森県,秋田県,山形県,岩手県,宮城県
｜      ｜    11系:小樽市,函館市,伊達市,北斗市,北海道後志総合振興局の所管区域,北海道胆振総合振興局の所管区域のうち豊浦町,壮瞥町及び洞爺湖町,北海道渡島総合振興局の所管区域,北海道檜山振興局の所管区域
｜      ｜    12系:北海道（XI系及びXIII系に規定する区域を除く。）
｜      ｜    13系:北見市,帯広市,釧路市,網走市,根室市,北海道オホーツク総合振興局の所管区域のうち美幌町,津別町,斜里町,清里町,小清水町,訓子府町,置戸町,佐呂間町及び大空町,北海道十勝総合振興局の所管区域,北海道釧路総合振興局の所管区域,北海道根室振興局の所管区域
｜      ｜    14系:東京都のうち北緯28度から南でありかつ東経140度30分から東であり東経143度から西である区域
｜      ｜    15系:沖縄県のうち東経126度から東でありかつ東経130度から西である区域
｜      ｜    16系:沖縄県のうち東経126度から西である区域
｜      ｜    17系:沖縄県のうち東経130度から東である区域
｜      ｜    18系:東京都のうち北緯28度から南でありかつ東経140度30分から西である区域
｜      ｜    19系:東京都のうち北緯28度から南であり、かつ東経143度から東である区域
｜      ｜  緯度(南北方向)
｜      ｜     3552.9400 →  35度52.9400分
｜      ｜  経度(東西方向)
｜      ｜    13926.9090 → 139度26.9090分
｜      ｜<<出力>>
｜      ｜  X座標(上下方向)
｜      ｜    -12986.342
｜      ｜  Y座標(左右方向)
｜      ｜    -34747.576
├------┼---------------------------------------------------------------------
｜ 履歴 ｜  日付     時刻   担当
｜作成日｜16/10/18 12:30:00 sasahara C#版
｜改定日｜
└------┴---------------------------------------------------------------------
                                             
*/
public class NEDOGPSHelper {

	//  01系～19系の原点北緯
	static double[] lat0List = {
		3300.0,
		3300.0,
		3600.0,
		3300.0,
		3600.0,
		3600.0,
		3600.0,
		3600.0,
		3600.0,
		4000.0,
		4400.0,
		4400.0,
		4400.0,
		2600.0,
		2600.0,
		2600.0,
		2600.0,
		2000.0,
		2600.0
	};
	//  01系～19系の原点東経
	static double[] lng0List = {
		12930.0,
		13100.0,
		13210.0,
		13330.0,
		13420.0,
		13600.0,
		13710.0,
		13830.0,
		13950.0,
		14050.0,
		14015.0,
		14215.0,
		14415.0,
		14200.0,
		12730.0,
		12400.0,
		13100.0,
		13600.0,
		15400.0
	};

	//  緯度経度⇒平面直角座標系変換
	public static void Calc_XY(int intKei, double dblIDOX, double dblKDOY, ref double dblX, ref double dblY)
	{
		try
		{
			intKei = intKei - 1;                        //  平面直角座標系
			double decLat = DMM2DEG(dblIDOX);           //  緯度
			double decLng = DMM2DEG(dblKDOY);           //  経度
			double decLat0 = DMM2DEG(lat0List[intKei]); //  緯度原点
			double decLng0 = DMM2DEG(lng0List[intKei]); //  経度原点
			//  ラジアン変換
			double radLat = decLat * Math.PI / 180.0;
			double radLng = decLng * Math.PI / 180.0;
			double radLat0 = decLat0 * Math.PI / 180.0;
			double radLng0 = decLng0 * Math.PI / 180.0;
			//  定数
			double a = 6378137.0;                       //  長半径(世界測地系)日本測地系:6377397.155
			double f = 1.0 / 298.257222101;             //  扁平率(世界測地系)日本測地系:1/299.152813
			double e1 = Math.Sqrt(2.0 * f - Math.Pow(f, 2.0));              //  第一離心率
			double e2 = Math.Sqrt(2.0 * 1.0 / f - 1.0) / (1.0 / f - 1.0);   //  第二離心率
			//  パラメータ
			double pA = 1.0 + 3.0 / 4.0 * Math.Pow(e1, 2.0) + 45.0 / 64.0 * Math.Pow(e1, 4.0) + 175.0 / 256.0 * Math.Pow(e1, 6.0) + 11025.0 / 16384.0 * Math.Pow(e1, 8.0) + 43659.0 / 65536.0 * Math.Pow(e1, 10.0) + 693693.0 / 1048576.0 * Math.Pow(e1, 12.0) + 19324305.0 / 29360128.0 * Math.Pow(e1, 14.0) + 4927697775.0 / 7516192768.0 * Math.Pow(e1, 16.0);
			double pB = 3.0 / 4.0 * Math.Pow(e1, 2.0) + 15.0 / 16.0 * Math.Pow(e1, 4.0) + 525.0 / 512.0 * Math.Pow(e1, 6.0) + 2205.0 / 2048.0 * Math.Pow(e1, 8.0) + 72765.0 / 65536.0 * Math.Pow(e1, 10.0) + 297297.0 / 262144.0 * Math.Pow(e1, 12.0) + 135270135.0 / 117440512.0 * Math.Pow(e1, 14.0) + 547521975.0 / 469762048.0 * Math.Pow(e1, 16.0);
			double pC = 15.0 / 64.0 * Math.Pow(e1, 4.0) + 105.0 / 256.0 * Math.Pow(e1, 6.0) + 2205.0 / 4096.0 * Math.Pow(e1, 8.0) + 10395.0 / 16384.0 * Math.Pow(e1, 10.0) + 1486485.0 / 2097152.0 * Math.Pow(e1, 12.0) + 45090045.0 / 58720256.0 * Math.Pow(e1, 14.0) + 766530765.0 / 939524096.0 * Math.Pow(e1, 16.0);
			double pD = 35.0 / 512.0 * Math.Pow(e1, 6.0) + 315.0 / 2048.0 * Math.Pow(e1, 8.0) + 31185.0 / 131072.0 * Math.Pow(e1, 10.0) + 165165.0 / 524288.0 * Math.Pow(e1, 12.0) + 45090045.0 / 117440512.0 * Math.Pow(e1, 14.0) + 209053845.0 / 469762048.0 * Math.Pow(e1, 16.0);
			double pE = 315.0 / 16384.0 * Math.Pow(e1, 8.0) + 3465.0 / 65536.0 * Math.Pow(e1, 10.0) + 99099.0 / 1048576.0 * Math.Pow(e1, 12.0) + 4099095.0 / 29360128.0 * Math.Pow(e1, 14.0) + 348423075.0 / 1879048192.0 * Math.Pow(e1, 16.0);
			double pF = 693.0 / 131072.0 * Math.Pow(e1, 10.0) + 9009.0 / 524288.0 * Math.Pow(e1, 12.0) + 4099095.0 / 117440512.0 * Math.Pow(e1, 14.0) + 26801775.0 / 469762048.0 * Math.Pow(e1, 16.0);
			double pG = 3003.0 / 2097152.0 * Math.Pow(e1, 12.0) + 315315.0 / 58720256.0 * Math.Pow(e1, 14.0) + 11486475.0 / 939524096.0 * Math.Pow(e1, 16.0);
			double pH = 45045.0 / 117440512.0 * Math.Pow(e1, 14.0) + 765765.0 / 469762048.0 * Math.Pow(e1, 16.0);
			double pI = 765765.0 / 7516192768.0 * Math.Pow(e1, 16.0);
			//  パラメータ計算
			double bCoef = a * (1.0 - Math.Pow(e1, 2.0));
			double b1 = bCoef * pA;
			double b2 = bCoef * -pB / 2.0;
			double b3 = bCoef * pC / 4.0;
			double b4 = bCoef * -pD / 6.0;
			double b5 = bCoef * pE / 8.0;
			double b6 = bCoef * -pF / 10.0;
			double b7 = bCoef * pG / 12.0;
			double b8 = bCoef * -pH / 14.0;
			double b9 = bCoef * pI / 16.0;
			//  赤道から座標系の原点の緯度までの子午線弧長
			double s0 = b1 * radLat0 + b2 * Math.Sin(2.0 * radLat0) + b3 * Math.Sin(4.0 * radLat0) + b4 * Math.Sin(6.0 * radLat0) + b5 * Math.Sin(8.0 * radLat0) + b6 * Math.Sin(10.0 * radLat0) + b7 * Math.Sin(12.0 * radLat0) + b8 * Math.Sin(14.0 * radLat0) + b9 * Math.Sin(16.0 * radLat0);
			//  赤道から入力緯度までの子午線弧長
			double s = b1 * radLat + b2 * Math.Sin(2.0 * radLat) + b3 * Math.Sin(4.0 * radLat) + b4 * Math.Sin(6.0 * radLat) + b5 * Math.Sin(8.0 * radLat) + b6 * Math.Sin(10.0 * radLat) + b7 * Math.Sin(12.0 * radLat) + b8 * Math.Sin(14.0 * radLat) + b9 * Math.Sin(16.0 * radLat);
			//  経度の差(東方を正)
			double deltaLambda = radLng - radLng0;
			double eta2 = Math.Pow(e2, 2.0) * Math.Pow(Math.Cos(radLat), 2.0);
			double t = Math.Tan(radLat);
			//  座標系の原点における縮尺係数
			double m0 = 0.9999;
			//  卯酉線曲率半径
			double w = Math.Sqrt(1.0 - Math.Pow(e1, 2.0) * Math.Pow(Math.Sin(radLat), 2.0));
			double n = a / w;
			//  X座標
			dblX = ((s - s0) + 1.0 / 2.0 * n * Math.Pow(Math.Cos(radLat), 2.0) * t * Math.Pow(deltaLambda, 2.0) + 1.0 / 24.0 * n * Math.Pow(Math.Cos(radLat), 4.0) * t * (5.0 - Math.Pow(t, 2.0) + 9.0 * eta2 + 4.0 * Math.Pow(eta2, 2.0)) * Math.Pow(deltaLambda, 4.0) - 1.0 / 720.0 * n * Math.Pow(Math.Cos(radLat), 6.0) * t * (-61.0 + 58.0 * Math.Pow(t, 2.0) - Math.Pow(t, 4.0) - 270.0 * eta2 + 330.0 * Math.Pow(t, 2.0) * eta2) * Math.Pow(deltaLambda, 6.0) - 1.0 / 40320.0 * n * Math.Pow(Math.Cos(radLat), 8.0) * t * (-1385.0 + 3111.0 * Math.Pow(t, 2.0) - 543.0 * Math.Pow(t, 4.0) + Math.Pow(t, 6.0)) * Math.Pow(deltaLambda, 8.0)) * m0;
			//  Y座標
			dblY = (n * Math.Cos(radLat) * deltaLambda - 1.0 / 6.0 * n * Math.Pow(Math.Cos(radLat), 3.0) * (-1.0 + Math.Pow(t, 2.0) - eta2) * Math.Pow(deltaLambda, 3.0) - 1.0 / 120.0 * n * Math.Pow(Math.Cos(radLat), 5.0) * (-5.0 + 18.0 * Math.Pow(t, 2.0) - Math.Pow(t, 4.0) - 14.0 * eta2 + 58.0 * Math.Pow(t, 2.0) * eta2) * Math.Pow(deltaLambda, 5.0) - 1.0 / 5040.0 * n * Math.Pow(Math.Cos(radLat), 7.0) * (-61.0 + 479.0 * Math.Pow(t, 2.0) - 179.0 * Math.Pow(t, 4.0) + Math.Pow(t, 6.0)) * Math.Pow(deltaLambda, 7.0)) * m0;
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
		}
	}

	//  度分.分⇒度換算
	private static double DMM2DEG(double dblDMM)
	{
		double dblD = 0d;
		double dblM = 0d;
		//  度の位へ、分の位÷60を足すと度換算となる
		try //  dddmm.mmmmmm
		{   //   度分 1/10000分
			//  全桁が60進数である
			string strDMM = dblDMM.ToString("00000.000000");
			dblD = double.Parse(strDMM.Substring(0, 3));    //  度(nnn00.000000)
			dblM = double.Parse(strDMM.Substring(3, 9));    //  分(000nn.000000)
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);

		}
		//  度変換
		return dblD + dblM / 60d;
	}

}
