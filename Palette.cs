using AshLib;
using AshLib.AshFiles;
using AshLib.Formatting;
using AshConsoleGraphics;

static class Palette{
	public static CharFormat? User {get; private set;}
	public static CharFormat? Static {get; private set;}
	public static CharFormat? Number {get; private set;}
	public static CharFormat? Continue {get; private set;}
	public static CharFormat? Error {get; private set;}
	public static CharFormat? Profit {get; private set;}
	public static CharFormat? Loss {get; private set;}
	public static CharFormat? Ash {get; private set;}
	
	public static void initialize(){
		AshFileModel m = new AshFileModel(
			new ModelInstance(ModelInstanceOperation.Type, "palette.user", Color3.Yellow),
			new ModelInstance(ModelInstanceOperation.Type, "palette.static", Color3.Yellow),
			new ModelInstance(ModelInstanceOperation.Type, "palette.number", new Color3("6BE4F4")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.continue", new Color3("C8FFB5")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.error", new Color3("D83F3C")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.profit", Color3.Green),
			new ModelInstance(ModelInstanceOperation.Type, "palette.loss", new Color3("FF4535")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.ash", new Color3("9150B2")),
			new ModelInstance(ModelInstanceOperation.Type, "useColors", true)
		);
		
		Finances.config *= m;
		
		if((!FormatString.usesColors) || (Finances.config.CanGetCamp("useColors", out bool b) && !b)){
			AshConsoleGraphics.Buffer.NoFormat = true; //Its (no longer) broken :)
			return;
		}
		
		AshConsoleGraphics.Buffer.NoFormat = false;
		
		User = new CharFormat(Finances.config.GetCamp<Color3>("palette.user"));
		Static = new CharFormat(Finances.config.GetCamp<Color3>("palette.static"));
		Number = new CharFormat(Finances.config.GetCamp<Color3>("palette.number"));
		Continue = new CharFormat(Finances.config.GetCamp<Color3>("palette.continue"));
		Error = new CharFormat(Finances.config.GetCamp<Color3>("palette.error"));
		Profit = new CharFormat(Finances.config.GetCamp<Color3>("palette.profit"));
		Loss = new CharFormat(Finances.config.GetCamp<Color3>("palette.loss"));
		Ash = new CharFormat(Finances.config.GetCamp<Color3>("palette.ash"));
		
		Finances.config.Save();
	}
}