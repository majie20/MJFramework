//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
	[JSONRootAttribute]
	[System.SerializableAttribute]
	public class APAnimation: APAsset 
	{
		[JSONDataMemberAttribute]
		public string InFile { get; set; }

		[JSONDataMemberAttribute]
		public float Length { get; set; }

		[JSONDataMemberAttribute]
		public int FPS { get; set; }

		[JSONDataMemberAttribute]
		public bool LoopTime { get; set; }

		[JSONDataMemberAttribute]
		public bool LoopPose { get; set; }

		[JSONDataMemberAttribute]
		public float CycleOffset { get; set; }

		public static string GetModelAnimationPath(string modelPath, long localId)
		{
			return string.Format("{0}?{1}", modelPath, localId);
		}

		public APAnimation()
		{
			APType = AssetType.ANIMATIONS;
		}
	}
}

#endif