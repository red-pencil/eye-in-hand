// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

public class GetBuilding_URL
{
    protected internal List<string> fileNameList = new List<string>();
    protected internal List<string> buildingCoordinate = new List<string>();

    public List<string> LocalFolderCheck()
    {
        List<string> CacheDataList = new List<string>();

        string path =  Application.streamingAssetsPath + "/Building";
			
			string[] files = Directory.GetFiles(path, "*.csy");
			
			  foreach (string _file in files)
            {
				string[] filter = _file.Split('/');
				string[] filter2 = filter[filter.Length-1].Split('\\');
				string filename = filter2[filter2.Length-1];
		        CacheDataList.Add(filename);
			}
        return CacheDataList;
    }

    void BuildingAssetSetting()
    {
        if (GameObject.Find("Building Assets") == null)
        {
            buildingAssets = new GameObject("Building Assets");
        }
        else
        {
            buildingAssets = GameObject.Find("Building Assets");
        }

    }
	
	public   WWW www_building;
	
    public IEnumerator Run(string phpurl)
    {
        WWWForm form = new WWWForm();
        form.AddField("state", "csy");
         www_building = new WWW(phpurl + "connect.php", form);
		 
        yield return www_building;
        if (www_building.error == null)
        {
            string longText = www_building.text;
            if (longText.Length != 0)
            {
                longText = longText.Substring(0, longText.LastIndexOf('^'));
                string[] fileName = longText.Split('^');
                for (int i = 0; i < fileName.Length; i++)
                {
                    fileNameList.Add(fileName[i]);
                    string[] _coordinate = fileName[i].Split('&');
                    buildingCoordinate.Add(_coordinate[1]);
                }
            }
        }
    }

    public void LocalRun()
    {
		  string path = Application.streamingAssetsPath + "/Building";
	      string[] files = Directory.GetFiles(path, "*.csy");
			  foreach (string _file in files)
            {
				string[] filter = _file.Split('/');
				string[] filter2 = filter[filter.Length-1].Split('\\');
				string filename = filter2[filter2.Length-1];
		        fileNameList.Add(filename);
                string[] _coordinate = filename.Split('&');
                buildingCoordinate.Add(_coordinate[1]);
			}
    }

    CalDistance calDis = new CalDistance();
    bool downloadComplete = false;
	
	
    public IEnumerator Call(string phpurl, string fileName, double[] target)
    {
        Manager mg = (Manager)GameObject.Find("Manager").GetComponent<Manager>();

if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.WebPlayer) {
        string buildingurl = phpurl + "Building/" + fileName;
        string outPath = Application.streamingAssetsPath + "/Building/" + fileName;

        if ((int)mg.sy_OtherOption.AssetBundle.ads == 0)   // Saving Binary File
        {
            if (CheckFileExists(fileName))
            {
                string[] strs = fileName.Split('&');
                string[] str = strs[1].Split(',');
                WWW www = new WWW(buildingurl);
                IEnumerator test = CheckProgress(www, str, target);
                while (test.MoveNext())
                {
                    yield return test.Current;
                }
                if (downloadComplete)
                {
                    if (www.error == null)
                    {
                        // Debug.Log("Downloading Modeling  :" +www.bytes.Length);
                        CreateBinaryFile(www.bytes, outPath);
                        IEnumerator bundle_file = LoadAssetBundle(www.bytes, fileName);
                        while (bundle_file.MoveNext())
                        {
                            yield return bundle_file.Current;
                        }
                    }
                }
            }
            else
            {
                if (mg.sy_Building.activated_building_name.Contains(fileName) == false)
                {
                    mg.sy_Building.activated_building_name.Add(fileName);
                    IEnumerator bundle_file = LoadAssetBundle(ReadFile(outPath), fileName);
                    while (bundle_file.MoveNext())
                    {
                        yield return bundle_file.Current;
                    }
                }
            }
        }
        else if ((int)mg.sy_OtherOption.AssetBundle.ads == 1)   //Saving Cashe File
        {
            if (CheckFileExists(fileName))
            {

                while (!Caching.ready)
                    yield return null;
                using (WWW www = WWW.LoadFromCacheOrDownload(buildingurl, 5))
                {
                    yield return www;

                    IEnumerator _loadCashe = LoadCashe(www, fileName);
                  while (_loadCashe.MoveNext())
                   {
                        yield return _loadCashe.Current;
                    }
                }

            }
        }
		}else{
			   string docPath = Application.dataPath + "/StreamingAssets/Building/"+fileName;
			   if (mg.sy_Building.activated_building_name.Contains(fileName) == false)
                {			
				mg.sy_Building.activated_building_name.Add(fileName);	
				
                  WWW www = new WWW (docPath);
		          yield return www;
				
		          if (www.error == null) {
					IEnumerator _loadCashe = LoadCashe(www, fileName);
					  while (_loadCashe.MoveNext())
                    {
                        yield return _loadCashe.Current;
                    }
			}
		}
		}
    }

    public void CreateBinaryFile(byte[] byteData, string path)
    {

        FileStream fs = new FileStream(path, FileMode.Create);
        fs.Seek(0, SeekOrigin.Begin);
        fs.Write(byteData, 0, byteData.Length);
        fs.Close();
    }

    public IEnumerator CheckProgress(WWW www, string[] str, double[] target)
    {

        downloadComplete = true;
        while (!www.isDone)
        {
            double dis = calDis.distance(target[1], target[0], double.Parse(str[1]), double.Parse(str[0]));
            if (dis > limits())
            {
                downloadComplete = false;
                www.Dispose();
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0f);
                //Debug.Log (Mathf.Round (www.progress) * 100 + "%");
            }
        }
    }

    bool CheckFileExists(string fileName)
    {
        Manager mg = (Manager)GameObject.Find("Manager").GetComponent<Manager>();

        if (mg.sy_Building.activated_building_name.Contains(fileName) == false)
        {
            if (mg.sy_Building.binary_filename.Contains(fileName) == false || (int)mg.sy_OtherOption.AssetBundle.ads == 1)
            {
                mg.sy_Building.activated_building_name.Add(fileName);
                mg.sy_Building.binary_filename.Add(fileName);
                //   Debug.Log("New Downloading");
                return true;
            }
            else
            {
                //          Debug.Log("Cache Data");
                return false;
            }
        }
        else
        {
            //         Debug.Log("Already Bundle");
            return false;
        }
    }

    Vector3 clickPos;

    public void CalDistance(bool state)
    {
        Manager mg = (Manager)GameObject.Find("Manager").GetComponent<Manager>();

        if (Input.GetMouseButtonDown(0))
            clickPos = Input.mousePosition;

        if (Input.GetMouseButtonUp(0) || state.Equals(true))
        {
            if ((clickPos - Input.mousePosition).magnitude > 200)
            {
                for (int k = 0; k < mg.sy_Building.coordinate.Count; k++)
                {
                    string[] str = mg.sy_Building.coordinate[k].Split(',');
                    double dis = calDis.distance(mg.sy_Coordinate.center[1], mg.sy_Coordinate.center[0], double.Parse(str[1]), double.Parse(str[0]));
                    if (limits() != 0 && Mathf.Abs((float)dis) < limits())
                    {
                        mg.ImportModeling(mg.sy_Building.name[k], mg.sy_Coordinate.center);
                    }
                    else
                    {
						
                        if (GameObject.Find("Building Assets") != null)
                        {
                            GameObject test = GameObject.Find("Building Assets");
                            for (int j = 0; j < test.transform.childCount; j++)
                            {
                                if (test.transform.GetChild(j).GetComponent<BuildingData>().fileName == mg.sy_Building.name[k])
                                {
                                    mg.sy_Building.activated_building_name.Remove(test.transform.GetChild(j).GetComponent<BuildingData>().fileName);
									test.transform.GetChild(j).GetComponent<BuildingData>().bundle.Unload(true);
                                    GameObject.DestroyImmediate(test.transform.GetChild(j).gameObject);
									break;
                                }
                            }
                        }
                        
                    }
                }
            }
        }
    }

    int limits()
    {
        Manager mg = (Manager)GameObject.Find("Manager").GetComponent<Manager>();
        int zoom = mg.sy_Map.zoom;
        int zoomMax = 20;
        for (int i = 0; i < 8; i++)
        {
            if (zoom == zoomMax - i)
            {
                return (int)mg.sy_OtherOption.Camera.distance[i];
            }
        }
        return 0;
    }

    GameObject buildingAssets;
    GameObject instance = null;

    public IEnumerator LoadAssetBundle(byte[] download, string fileName)
    {
        Manager mg = GameObject.Find("Manager").GetComponent<Manager>();
		a_zoom = mg.sy_Map.zoom;
        BuildingAssetSetting();
		
	   	yield return new WaitForSeconds(0.8f);	
		if(mg.sy_CurrentStatus.is3DCam && a_zoom == mg.sy_Map.zoom && mg.sy_Map.zoom >12){
            AssetBundleCreateRequest bundle = AssetBundle.LoadFromMemoryAsync(download);
            yield return bundle;
			
			 AssetBundle assetBundle = bundle.assetBundle;
            //AssetBundleRequest abr = assetBundle.LoadAsync(assetBundle.mainAsset.name, typeof(GameObject)); 
			AssetBundleRequest abr = assetBundle.LoadAssetAsync(assetBundle.mainAsset.name, typeof(GameObject));
			yield return abr;
            instance = GameObject.Instantiate(abr.asset) as GameObject;
            instance.name = assetBundle.mainAsset.name;
           instance.GetComponent<BuildingData>().fileName = fileName;
	       instance.GetComponent<BuildingData>().sy_zoom = mg.sy_Map.zoom;
			instance.GetComponent<BuildingData>().bundle =assetBundle;
            instance.transform.parent = buildingAssets.transform;

		}
	else{
			IEnumerator reLoad = LoadAssetBundle(download, fileName);
					  while (reLoad.MoveNext())
                    {
                        yield return reLoad.Current;
                    }
		}
    }
	
	int a_zoom;
	
    public IEnumerator LoadCashe(WWW www, string fileName)
    {
		
        Manager mg = GameObject.Find("Manager").GetComponent<Manager>();
		a_zoom = mg.sy_Map.zoom;
        BuildingAssetSetting();
		yield return new WaitForSeconds(0.8f);
		
		if(mg.sy_CurrentStatus.is3DCam && a_zoom == mg.sy_Map.zoom && mg.sy_Map.zoom >12){
           	AssetBundle assetBundle = www.assetBundle;
          //  AssetBundleRequest abr = assetBundle.LoadAsync(assetBundle.mainAsset.name, typeof(GameObject));
          //yield return abr;
            instance = GameObject.Instantiate(assetBundle.mainAsset) as GameObject;
            instance.name = assetBundle.mainAsset.name;
            instance.GetComponent<BuildingData>().fileName = fileName;
			instance.GetComponent<BuildingData>().sy_zoom = mg.sy_Map.zoom;
			instance.GetComponent<BuildingData>().bundle =assetBundle;
            instance.transform.parent = buildingAssets.transform;
			
		}
				else{
	       			IEnumerator reLoad = LoadCashe(www, fileName);
					  while (reLoad.MoveNext())
                    {
                        yield return reLoad.Current;
                    }
	}
    }


    public static byte[] ReadFile(string filePath)
    {
        byte[] buffer;
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        try
        {
            int length = (int)fileStream.Length;
            buffer = new byte[length];
            int count;
            int sum = 0;
            while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                sum += count;
        }
        finally
        {
            fileStream.Flush();
            fileStream.Close();
        }
        return buffer;
    }
	
}
