<?php



$state = $_POST["state"];

$address = $_POST["address"];

$imgurl = $_POST['mapurl'];


/////////////////////////////////
//Getting filenames in directory
/////////////////////////////////

if($state =='xml'){
$dirpath = './Mark';
}
else if($state =='csy'){
$dirpath = './Building';
}

if($state !=""){

$dirPath = dir($dirpath);

$FileArray = array();

while (($file = $dirPath->read()) !== false)
{

  if ((substr($file, -3)==$state))
  {
     $FileArray[ ] = trim($file);
  }
}

$dirPath->close();

sort($FileArray);



$co = count($FileArray);


for($i=0; $i<$co; $i++)
{
   echo $FileArray[$i].'^';
}

}


/////////////////////////////////
//GeoCoding
/////////////////////////////////

if($address !=""){
$url = "http://maps.google.com/maps/api/geocode/json?address=$address&sensor=false";
$ch = curl_init();
curl_setopt($ch, CURLOPT_URL, $url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
curl_setopt($ch, CURLOPT_PROXYPORT, 3128);
curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 0);
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, 0);
$response = curl_exec($ch);
curl_close($ch);
$response_a = json_decode($response);


//print_r( $response_a);

echo $long = $response_a->results[0]->geometry->location->lng;
echo "&";
echo $lat = $response_a->results[0]->geometry->location->lat;
echo "&";
echo $geo_address = $response_a->results[0]->formatted_address;
}



if($imgurl !=""){
//////////////////////////////
// get Unity3d Data
//////////////////////////////

  $byteSize =  (int)$_POST['byteSize'];

//////////////////////////////
//get URL image data
////////////////////////////// 

   function getImageData($imgLink)
   {

    $curl = curl_init(); 
    curl_setopt($curl, CURLOPT_URL, $imgLink);
    curl_setopt($curl, CURLOPT_TIMEOUT,0.8);
	curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, 0);
     curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, 0);
    $data = curl_exec($curl);
    curl_close($curl);
    return $data;    
   }


//////////////////////////////
//Create Image
//////////////////////////////


    $data = getImageData($imgurl);

    //sleep($interval);
	if (strlen($data) <$byteSize){
      die;
	}
	else{

    $im = imagecreatefromstring($data);
    imagejpeg($im);
    imagedestroy($im); 
	}

}
	
?>