// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using  System.Text.RegularExpressions;

public class AutoComplete
{
	List<string> name = new List<string> ();
	List<string> overlap = new List<string> ();
	List<string> target_list = new List<string> ();

	protected internal string[] Run (string content, string idleText, string[][] targetsPlus)
	{
		overlap.Clear ();
		name.Clear ();
		string[] contents = content.Split (' ');

		if (content != idleText) {
			
			if (targetsPlus != null)
				for (int p=0; p< targetsPlus.Length; p++) {
					for (int y=0; y< targetsPlus[p].Length; y++) {
						target_list.Add (targetsPlus [p] [y]);
					}
				}
			
			string[] targets = target_list.ToArray ();
			
			
			foreach (string target in targets) {
				string[] _targets = target.Split (' ');
				
				for (int k=0; k<_targets.Length; k++) {
					string ori = _targets [k];
					string first = ori [0].ToString ();
					string first_con = content [0].ToString ();
					
					MatchCollection first_matches = Regex.Matches (first.ToLower (), first_con.ToLower ());
					
					if (first_matches.Count != 0) {
						if (k == 0) {
							if (ori.ToLower ().Contains (content.ToLower ()) == true) {
								if (!overlap.Contains (target)) {
									overlap.Add (target);
								}
							}
						} else {
							if (content.Length > 2) {
								if (ori.ToLower ().Contains (content.ToLower ()) == true) {
									if (!overlap.Contains (target)) {
										overlap.Add (target);
									}
								}
							}
						}
					}	
					
					if (contents [0].Length > 0 && contents.Length > 1) {
						
						MatchCollection Second_matches = Regex.Matches (target.ToLower (), content.ToLower ());	
						if (Second_matches.Count != 0) {
							//	Debug.Log (target + " , " + content);	
							if (!overlap.Contains (target)) {
								overlap.Add (target);
							}
						}
					}
				}	
			}
		}

		return overlap.ToArray ();
		
	}
}
