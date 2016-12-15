using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Web.Configuration;
using System.Windows.Forms;
using RSM.Artifacts;
using RSM.Integration.S2;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model;
using RSM.Staging.Library;
using RSM.Staging.Library.Data;
using RSM.Support;
using ExternalSystem = RSM.Support.ExternalSystem;
using Factory = RSM.Staging.Library.Factory;
using Location = RSM.Support.Location;

namespace TestSupport
{
	public partial class Staging : Form
	{
		#region Properties
		public RSMDataModelDataContext Context { get; set; }

		public string ValidationKey { get; set; }

		public ExternalSystem S2 { get; set; }

		public ExternalSystem Track { get; set; }

		#endregion

		public Staging()
		{
			InitializeComponent();

			Context = new RSMDataModelDataContext(Constants.ConnectionStringName);

			var machineKey = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");

			ValidationKey = machineKey.ValidationKey;

			S2 = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.S2In.Id);
			Track = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.TrackOut.Id);
		}

		#region Settings
		private void SettingsClick(object sender, EventArgs e)
		{
			if (includeSettingR1SM.Checked)
			{
				R1SMSettingsClick(sender, e);
			}

			if (includeSettingsS2Importer.Checked)
			{
				S2InSettingsClick(sender, e);
			}

			if (includeSettingsTrackExporter.Checked)
			{
				TrackOutSettingsClick(sender, e);
			}

			if (includeSettingsLubrizolImporter.Checked)
			{
				LubrizolInSettingsClick(sender, e);
			}

			if (includeSettingsLubrizolExporter.Checked)
			{
				LubrizolOutSettingsClick(sender, e);
			}
		}

		private void TrackOutSettingsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Track Settings Stage", true);

			var factory = new DataLoad();

			factory.TrackSettings(ValidationKey);

			DisplayStatus("Track Settings staging completed", false);
		}

		private void S2InSettingsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting S2Import Settings Stage", true);

			var factory = new DataLoad();

			factory.S2InSettings(ValidationKey);

			DisplayStatus("S2 Settings staging completed", false);
		}

		private void R1SMSettingsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting R1SM Settings Stage", true);

			var factory = new DataLoad();

			factory.R1SMSettings(ValidationKey);

			DisplayStatus("R1SM Settings staging completed", false);
		}

		private void LubrizolOutSettingsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Lubrizol Export Settings Stage", true);

			var factory = new DataLoad();

			factory.LubrizolOutSettings(ValidationKey);

			DisplayStatus("Lubrizol Export Settings staging completed", false);
		}

		private void LubrizolInSettingsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Lubrizol Import Settings Stage", true);

			var factory = new DataLoad();

			factory.LubrizolInSettings(ValidationKey);

			DisplayStatus("Lubrizol Import Settings staging completed", false);
		}
		#endregion

		#region Data Staging
		private void StageExternalSystemsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Systems Stage", true);

			Context.Refresh(RefreshMode.KeepCurrentValues);

			Factory.Delete(Context, ExternalSystems.R1SM);

			var r1sm = ExternalSystems.R1SM;
			r1sm.Settings = null;
			r1sm.ExternalApplicationKeys = null;
			Context.ExternalSystems.InsertOnSubmit(r1sm);

			Factory.Delete(Context, ExternalSystems.S2In);

			var s2In = ExternalSystems.S2In;
			s2In.Settings = null;
			s2In.ExternalApplicationKeys = null;
			Context.ExternalSystems.InsertOnSubmit(s2In);
			S2 = s2In;

			Factory.Delete(Context, ExternalSystems.TrackOut);

			var trackOut = ExternalSystems.TrackOut;
			trackOut.Settings = null;
			trackOut.ExternalApplicationKeys = null;
			Context.ExternalSystems.InsertOnSubmit(trackOut);
			Track = trackOut;

			Context.SubmitChanges();

			DisplayStatus("External Systems staging completed", false);
		}

		private void CreatePerson(Factory factory, string firstName, string lastName, string externalId = null, string badgeNumber = null, bool isContractor = true, bool isAdmin = false, string username = null, string password = null)
		{
			var udfs = isContractor ? new Dictionary<int, string>()
			           	{
			           		{4, "Contractor Company"}
			           	} : null;

			var person = factory.createPerson(firstName, lastName, middleName: null, id: null, isAdmin: isAdmin, isLockedOut: false,
			                     username: username, password: Factory.EncryptPassword(password, ValidationKey),
			                     encryptPassword: false, credentials: null, nickname: null, badgeNumber: badgeNumber,
			                     externalId: externalId, system: S2, UDFs: udfs, action: EntityAction.InsertAndSubmit);

			DisplayStatus(string.Format("Person '{0}' added", person.DisplayName), false);
		}

		private void StagePeopleClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting People Stage", true);

			var factory = new Factory(Context);

			CreatePerson(factory, "System", "Administrator", externalId: "00001", badgeNumber: "", isContractor: false, isAdmin: true, 
					 username: "admin", password: "LubR1z0lTr@6K");

			CreatePerson(factory, "ERIC", "ADAMS", externalId: "1026473", badgeNumber: "28469");
			CreatePerson(factory, "DARREN", "ADAMSON", externalId: "1047118", badgeNumber: "28475");
			CreatePerson(factory, "ESTEBAN", "AGUILAR", externalId: "1046729", badgeNumber: "");
			CreatePerson(factory, "ISRAEL", "AGUILAR", externalId: "1046383", badgeNumber: "24647");
			CreatePerson(factory, "JUAN", "AGUILAR", externalId: "1043968", badgeNumber: "24099");
			CreatePerson(factory, "OSCAR", "AGUIRRE", externalId: "1047539", badgeNumber: "28548");
			CreatePerson(factory, "RUPERTO", "ALANIS", externalId: "1047236", badgeNumber: "28241");
			CreatePerson(factory, "RUPERTO", "ALANIS", externalId: "", badgeNumber: "28034");
			CreatePerson(factory, "VICTOR", "ALANIZ", externalId: "1043263", badgeNumber: "");
			CreatePerson(factory, "VICTOR", "ALEGRIA", externalId: "1044177", badgeNumber: "");
			CreatePerson(factory, "JUBENAL", "ALEGRIA, JR.", externalId: "1046730", badgeNumber: "");
			CreatePerson(factory, "MICHAEL", "ALFARO", externalId: "1045442", badgeNumber: "27974");
			CreatePerson(factory, "PAUL", "ALFRED", externalId: "1025856", badgeNumber: "24842");
			CreatePerson(factory, "ASHTON", "ALLBROOK", externalId: "1047718", badgeNumber: "28088");
			CreatePerson(factory, "PAUL", "ALTMAN", externalId: "1044710", badgeNumber: "");
			CreatePerson(factory, "ALEJANDRO", "ALVARADO", externalId: "1047273", badgeNumber: "");
			CreatePerson(factory, "ADAM", "ALVAREZ", externalId: "1035810", badgeNumber: "18081");
			CreatePerson(factory, "MAGDA", "ALVAREZ", externalId: "", badgeNumber: "28298");
			CreatePerson(factory, "MAGDALENA", "ALVAREZ", externalId: "1042299", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "ALVEREZ", externalId: "1044433", badgeNumber: "24580");
			CreatePerson(factory, "MILTON", "AMAYA", externalId: "1046547", badgeNumber: "28115");
			CreatePerson(factory, "JUAN", "AMEZCUA", externalId: "1048011", badgeNumber: "28765");
			CreatePerson(factory, "ORLANDO", "ANARIBA", externalId: "1044429", badgeNumber: "");
			CreatePerson(factory, "JORGE", "ANDRADE", externalId: "1046732", badgeNumber: "");
			CreatePerson(factory, "DAVID", "ANDREWS", externalId: "1042452", badgeNumber: "");
			CreatePerson(factory, "ROLLAND", "ANGELL", externalId: "1046721", badgeNumber: "28170");
			CreatePerson(factory, "CHRISTOPHER", "APPLEBY", externalId: "1043908", badgeNumber: "");
			CreatePerson(factory, "GILBERTO", "ARAMBUL", externalId: "1045770", badgeNumber: "24501");
			CreatePerson(factory, "PEDRO", "ARAMBULA", externalId: "1027995", badgeNumber: "14826");
			CreatePerson(factory, "QUINTEL", "ARCHIE", externalId: "1047985", badgeNumber: "28707");
			CreatePerson(factory, "FELIPE", "ARELLANO", externalId: "1042160", badgeNumber: "28287");
			CreatePerson(factory, "FRANCISO", "ARGUELLO", externalId: "1046788", badgeNumber: "");
			CreatePerson(factory, "MICHAEL", "ARITA", externalId: "1045965", badgeNumber: "");
			CreatePerson(factory, "JEROME", "ARMSTRONG", externalId: "1032914", badgeNumber: "28216~28315");
			CreatePerson(factory, "ERNESTO", "ARREDONDO", externalId: "1042324", badgeNumber: "24036");
			CreatePerson(factory, "BALDEMAR", "ARREOLA", externalId: "1020356", badgeNumber: "3184");
			CreatePerson(factory, "ANTONIO", "ARZOLA-ALMANZA", externalId: "1044494", badgeNumber: "");
			CreatePerson(factory, "JAMES", "ASHFORD", externalId: "1020457", badgeNumber: "2491");
			CreatePerson(factory, "AINSLEY", "ATHERLEY", externalId: "1030562", badgeNumber: "");
			CreatePerson(factory, "CODY", "ATHEY", externalId: "1038091", badgeNumber: "24759~24505");
			CreatePerson(factory, "HERMENEGILDO", "AVILA", externalId: "1035829", badgeNumber: "18859~24628");
			CreatePerson(factory, "MIGUEL", "AVILA", externalId: "1043281", badgeNumber: "18814~28149");
			CreatePerson(factory, "MARK", "BABINEAUX", externalId: "1031005", badgeNumber: "14501~28183");
			CreatePerson(factory, "BRUCE", "BAILEY", externalId: "", badgeNumber: "");
			CreatePerson(factory, "FRANK", "BAKER", externalId: "1034725", badgeNumber: "24114");
			CreatePerson(factory, "TODD", "BAKER", externalId: "1044711", badgeNumber: "");
			CreatePerson(factory, "MARK", "BALDAZO", externalId: "1043262", badgeNumber: "28410");
			CreatePerson(factory, "GERALD", "BALDWIN", externalId: "1043131", badgeNumber: "");
			CreatePerson(factory, "MASON", "BALL", externalId: "1042240", badgeNumber: "");
			CreatePerson(factory, "ADAN", "BANDA", externalId: "1044482", badgeNumber: "");
			CreatePerson(factory, "WILL", "BANSKSTON", externalId: "1047087", badgeNumber: "28474");
			CreatePerson(factory, "JESUS", "BARAHONA", externalId: "1047567", badgeNumber: "28517");
			CreatePerson(factory, "FRED", "BARCLAY", externalId: "1046093", badgeNumber: "27983");
			CreatePerson(factory, "VINCENT", "BARGANSKI", externalId: "1043563", badgeNumber: "");
			CreatePerson(factory, "MONTY", "BARNES", externalId: "1045649", badgeNumber: "24813");
			CreatePerson(factory, "ERNEST", "BARRIENTES", externalId: "1044537", badgeNumber: "24596");
			CreatePerson(factory, "RENATO", "BARRON", externalId: "1039574", badgeNumber: "14513");
			CreatePerson(factory, "NICOLAS", "BAUTISTA", externalId: "1045650", badgeNumber: "");
			CreatePerson(factory, "DEBRA", "BAXTER", externalId: "1045595", badgeNumber: "");
			CreatePerson(factory, "DANIEL", "BENAVIDEZ", externalId: "1046787", badgeNumber: "");
			CreatePerson(factory, "CHARLES", "BENNETT", externalId: "1044701", badgeNumber: "");
			CreatePerson(factory, "WILLIAM", "BENNETT", externalId: "1047848", badgeNumber: "");
			CreatePerson(factory, "FERNANDO", "BERNAL", externalId: "1044516", badgeNumber: "24538");
			CreatePerson(factory, "BETTY", "BERRY", externalId: "1027108", badgeNumber: "781");
			CreatePerson(factory, "CHRISTOPHER", "BERRY", externalId: "1045322", badgeNumber: "");
			CreatePerson(factory, "EULALIO", "BERUMEN", externalId: "1044319", badgeNumber: "");
			CreatePerson(factory, "MITCHELL", "BIANO", externalId: "1047540", badgeNumber: "28490");
			CreatePerson(factory, "CLAYTON", "BLACHLY", externalId: "1044526", badgeNumber: "");
			CreatePerson(factory, "JULIAN", "BOCANEGRA", externalId: "1047987", badgeNumber: "28712");
			CreatePerson(factory, "DOUGLAS", "BODNAR", externalId: "1046378", badgeNumber: "");
			CreatePerson(factory, "ANTHONY", "BOHANNON", externalId: "1042876", badgeNumber: "");
			CreatePerson(factory, "JAMES", "BOWEN", externalId: "1022442", badgeNumber: "");
			CreatePerson(factory, "LEONARD", "BOWERS", externalId: "1027831", badgeNumber: "13953");
			CreatePerson(factory, "JOSEPH", "BOYD", externalId: "1020458", badgeNumber: "");
			CreatePerson(factory, "CHRISTOPHER", "BOYKIN", externalId: "1044657", badgeNumber: "");
			CreatePerson(factory, "ISMAR", "BRICENO", externalId: "1044715", badgeNumber: "");
			CreatePerson(factory, "RICKY", "BRIGHTWELL", externalId: "1025624", badgeNumber: "24104");
			CreatePerson(factory, "GILBERTO", "BRIONES", externalId: "1046376", badgeNumber: "");
			CreatePerson(factory, "RANDY", "BRITO", externalId: "1046963", badgeNumber: "28309");
			CreatePerson(factory, "TIMOTHY", "BROMBERK", externalId: "1031708", badgeNumber: "");
			CreatePerson(factory, "ELDWIN", "BROOKS", externalId: "1046420", badgeNumber: "");
			CreatePerson(factory, "JASON", "BROWN", externalId: "1042832", badgeNumber: "18449");
			CreatePerson(factory, "ROYTRAEL", "BROWN", externalId: "", badgeNumber: "");
			CreatePerson(factory, "WILLARD", "BUCK", externalId: "1025872", badgeNumber: "");
			CreatePerson(factory, "WILLIAM", "BURCHFIELD", externalId: "1047235", badgeNumber: "");
			CreatePerson(factory, "BART", "BURNS", externalId: "", badgeNumber: "");
			CreatePerson(factory, "ELIJAH", "BURNS", externalId: "1043239", badgeNumber: "");
			CreatePerson(factory, "LEVENIA", "BURNS", externalId: "1044438", badgeNumber: "24547");
			CreatePerson(factory, "ALSTON", "BURRELL", externalId: "", badgeNumber: "");
			CreatePerson(factory, "CLARENCE", "BURTON", externalId: "1045346", badgeNumber: "");
			CreatePerson(factory, "CLARENCE", "BURTON", externalId: "1033871", badgeNumber: "");
			CreatePerson(factory, "STEPHANIE", "CADENA", externalId: "1045842", badgeNumber: "27989");
			CreatePerson(factory, "DAVID", "CALVA", externalId: "1042680", badgeNumber: "");
			CreatePerson(factory, "ANGEL", "CAMPOS", externalId: "1047957", badgeNumber: "28795");
			CreatePerson(factory, "JOSE", "CANTU", externalId: "1045648", badgeNumber: "");
			CreatePerson(factory, "ERIC", "CANTU-MATA", externalId: "1043337", badgeNumber: "");
			CreatePerson(factory, "COY", "CAPPS", externalId: "1033178", badgeNumber: "3174");
			CreatePerson(factory, "JULIO", "CARDENAS", externalId: "1047480", badgeNumber: "28400");
			CreatePerson(factory, "JUAN", "CARILLO", externalId: "1047843", badgeNumber: "28717");
			CreatePerson(factory, "ANDRES", "CARMONA", externalId: "1044426", badgeNumber: "24509");
			CreatePerson(factory, "CESAR", "CARRILLO", externalId: "10433837", badgeNumber: "");
			CreatePerson(factory, "DARROW", "CARTER", externalId: "1033540", badgeNumber: "24488");
			CreatePerson(factory, "ARMANDO", "CASARES JR.", externalId: "1047089", badgeNumber: "28492");
			CreatePerson(factory, "JOSHUA", "CASSIDY", externalId: "1042162", badgeNumber: "");
			CreatePerson(factory, "ZACHARY", "CASSIDY", externalId: "1043079", badgeNumber: "");
			CreatePerson(factory, "GERMAIN", "CASTILLO", externalId: "1047944", badgeNumber: "28797");
			CreatePerson(factory, "VICTOR", "CASTILLO", externalId: "1044298", badgeNumber: "24452");
			CreatePerson(factory, "HEATHER", "CASTLE", externalId: "1047492", badgeNumber: "28401");
			CreatePerson(factory, "RODNEY", "CASTLE", externalId: "1043363", badgeNumber: "");
			CreatePerson(factory, "JOHN", "CASTLEBERRY", externalId: "1042795", badgeNumber: "");
			CreatePerson(factory, "RICARDO", "CAVAZOS", externalId: "1046894", badgeNumber: "28321");
			CreatePerson(factory, "VICTOR", "CERVANTES", externalId: "1044435", badgeNumber: "");
			CreatePerson(factory, "ALDO", "CHAVEZ", externalId: "1044527", badgeNumber: "24521");
			CreatePerson(factory, "HERIBERTO", "CHAVEZ", externalId: "1047919", badgeNumber: "28779");
			CreatePerson(factory, "LUIS", "CHAVEZ", externalId: "1042855", badgeNumber: "");
			CreatePerson(factory, "JOSE", "CHICAS", externalId: "1041502", badgeNumber: "");
			CreatePerson(factory, "JORGEN", "CHRISTOFFERSEN", externalId: "1042987", badgeNumber: "23928");
			CreatePerson(factory, "PATRICK", "CISNEROS", externalId: "1047968", badgeNumber: "28764");
			CreatePerson(factory, "RICHARD", "CISNEROS", externalId: "1045321", badgeNumber: "");
			CreatePerson(factory, "RUDY", "CISNEROS", externalId: "1045624", badgeNumber: "");
			CreatePerson(factory, "SAMUEL", "CISNEROS", externalId: "1044520", badgeNumber: "");
			CreatePerson(factory, "ALVIN", "CLARKE", externalId: "1047500", badgeNumber: "28577");
			CreatePerson(factory, "ELLIOTT", "COCHRAN", externalId: "1046749", badgeNumber: "");
			CreatePerson(factory, "JOSE", "COLCHADO", externalId: "1032478", badgeNumber: "");
			CreatePerson(factory, "IRVINE", "COLLINS", externalId: "1047147", badgeNumber: "28409");
			CreatePerson(factory, "TROY", "CORDERO", externalId: "1044500", badgeNumber: "");
			CreatePerson(factory, "HECTOR", "CORTEZ", externalId: "1046499", badgeNumber: "");
			CreatePerson(factory, "JOSE", "CORTEZ", externalId: "1033287", badgeNumber: "");
			CreatePerson(factory, "FRANK", "CORTINAS", externalId: "1047237", badgeNumber: "");
			CreatePerson(factory, "MICHAEL", "COX", externalId: "1044301", badgeNumber: "");
			CreatePerson(factory, "JASON", "CRAIG", externalId: "104/7715", badgeNumber: "");
			CreatePerson(factory, "JERROD", "CROCKER", externalId: "1047717", badgeNumber: "");
			CreatePerson(factory, "CLAYTON", "CROSS", externalId: "1046567", badgeNumber: "");
			CreatePerson(factory, "NATHANIEL", "CRUZ", externalId: "1044702", badgeNumber: "");
			CreatePerson(factory, "JAIME", "CRUZ-CERVANTES", externalId: "", badgeNumber: "28791");
			CreatePerson(factory, "VIC", "CRUZ-MARTINEZ", externalId: "1046705", badgeNumber: "28248");
			CreatePerson(factory, "JONATHAN", "CUNNINGHAM", externalId: "1047070", badgeNumber: "");
			CreatePerson(factory, "ALVIN", "CUPID", externalId: "1043487", badgeNumber: "");
			CreatePerson(factory, "JOSEMON", "DANIEL", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JAIME", "DAVID", externalId: "1047251", badgeNumber: "28505~28766");
			CreatePerson(factory, "MATTHEW", "DAVIS", externalId: "1044543", badgeNumber: "");
			CreatePerson(factory, "RONNIE", "DAVIS", externalId: "_13619", badgeNumber: "");
			CreatePerson(factory, "JOHN", "DEHAY", externalId: "1032898", badgeNumber: "18730");
			CreatePerson(factory, "CARLOS", "DELEON", externalId: "1039515", badgeNumber: "18083");
			CreatePerson(factory, "DAVID", "DELEON", externalId: "1047066", badgeNumber: "28489");
			CreatePerson(factory, "JUAN", "DELEON", externalId: "1036003", badgeNumber: "");
			CreatePerson(factory, "ANDRES", "DELGADO", externalId: "1043028", badgeNumber: "18648~24648");
			CreatePerson(factory, "ARNOLDO", "DELGADO", externalId: "1042372", badgeNumber: "28142");
			CreatePerson(factory, "FERNANO", "DELGADO", externalId: "1047494", badgeNumber: "28435");
			CreatePerson(factory, "JOEL", "DELGADO", externalId: "1042818", badgeNumber: "");
			CreatePerson(factory, "LARRY", "DELMAR", externalId: "1044619", badgeNumber: "");
			CreatePerson(factory, "Guillermo", "DeLosSantos", externalId: "1042796", badgeNumber: "");
			CreatePerson(factory, "RALPH", "DEL RIO", externalId: "1020549", badgeNumber: "711");
			CreatePerson(factory, "ISAIS", "DENA", externalId: "1041573", badgeNumber: "18645");
			CreatePerson(factory, "RAY", "DE SIO", externalId: "1046237", badgeNumber: "");
			CreatePerson(factory, "JOEL", "DIAZ", externalId: "1043969", badgeNumber: "24729");
			CreatePerson(factory, "PABLO", "DIAZ", externalId: "", badgeNumber: "");
			CreatePerson(factory, "PABLO", "DIAZ", externalId: "", badgeNumber: "28262");
			CreatePerson(factory, "ROBERT", "DICKSON", externalId: "1042817", badgeNumber: "23927");
			CreatePerson(factory, "MOREL", "DOMINGUEZ", externalId: "1047310", badgeNumber: "");
			CreatePerson(factory, "LEETRON", "EDWARDS", externalId: "1035895", badgeNumber: "");
			CreatePerson(factory, "FABIAN", "ELLIOTT", externalId: "1043428", badgeNumber: "24298");
			CreatePerson(factory, "JESUS", "ESTRADA", externalId: "1045942", badgeNumber: "");
			CreatePerson(factory, "DANIEL", "FARRAR", externalId: "1045628", badgeNumber: "");
			CreatePerson(factory, "AMERICO", "FERNANDEZ", externalId: "1046568", badgeNumber: "28145");
			CreatePerson(factory, "EVERARDO", "FERNANDEZ", externalId: "1046372", badgeNumber: "27948");
			CreatePerson(factory, "LLOVON", "FERNANDEZ", externalId: "1047033", badgeNumber: "");
			CreatePerson(factory, "OMAR", "FERNANDEZ", externalId: "1044441", badgeNumber: "");
			CreatePerson(factory, "ROBERTO", "FERRO", externalId: "1046417", badgeNumber: "");
			CreatePerson(factory, "CLIFTON", "FIKES", externalId: "1046733", badgeNumber: "");
			CreatePerson(factory, "JOSHUA", "FINCHER", externalId: "1027376", badgeNumber: "18649");
			CreatePerson(factory, "PIERRE", "FLEMING", externalId: "1043148", badgeNumber: "18616");
			CreatePerson(factory, "DAVID", "FLORES", externalId: "", badgeNumber: "");
			CreatePerson(factory, "DAVID", "FLORES", externalId: "1042370", badgeNumber: "27972");
			CreatePerson(factory, "ERIK", "FLORES", externalId: "1046418", badgeNumber: "27998");
			CreatePerson(factory, "JUVENTINO", "FLORES", externalId: "1036367", badgeNumber: "10223");
			CreatePerson(factory, "RUDY H.", "FLORES", externalId: "1047591", badgeNumber: "28570");
			CreatePerson(factory, "STEVE", "FLORES", externalId: "1039752", badgeNumber: "");
			CreatePerson(factory, "CHRISTOPHER", "FLY", externalId: "1038343", badgeNumber: "14251");
			CreatePerson(factory, "HOWARD", "FONSECA", externalId: "1041956", badgeNumber: "");
			CreatePerson(factory, "TERRACE", "FONTENOT", externalId: "1037663", badgeNumber: "");
			CreatePerson(factory, "CEDRIC", "FORIEST", externalId: "1046817", badgeNumber: "28172");
			CreatePerson(factory, "TERRY", "FOSTER", externalId: "1045476", badgeNumber: "");
			CreatePerson(factory, "TONY", "GALVAN", externalId: "1030359", badgeNumber: "17977");
			CreatePerson(factory, "ALBERT", "GARCIA", externalId: "1047028", badgeNumber: "");
			CreatePerson(factory, "AMADIO", "GARCIA", externalId: "1046374", badgeNumber: "");
			CreatePerson(factory, "EDGAR", "GARCIA", externalId: "1044509", badgeNumber: "");
			CreatePerson(factory, "FELIPE", "GARCIA", externalId: "104", badgeNumber: "18688~28466");
			CreatePerson(factory, "FERMIN", "GARCIA", externalId: "1047716", badgeNumber: "28585");
			CreatePerson(factory, "JESSE", "GARCIA", externalId: "1046789", badgeNumber: "28239");
			CreatePerson(factory, "JORGE", "GARCIA", externalId: "1043338", badgeNumber: "");
			CreatePerson(factory, "ROBERT", "GARCIA", externalId: "1031831", badgeNumber: "");
			CreatePerson(factory, "SAMUEL", "GARCIA", externalId: "1044695", badgeNumber: "");
			CreatePerson(factory, "GORGE", "GARCIA-CHAVEZ", externalId: "1044427", badgeNumber: "");
			CreatePerson(factory, "ALBERTO", "GARCILAZO", externalId: "1047274", badgeNumber: "");
			CreatePerson(factory, "ARTURO", "GARIBAY", externalId: "1047022", badgeNumber: "");
			CreatePerson(factory, "ARMANDO", "GARZA", externalId: "1020551", badgeNumber: "24819");
			CreatePerson(factory, "HECTOR", "GARZA", externalId: "1046571", badgeNumber: "");
			CreatePerson(factory, "JOSE", "GARZA", externalId: "1047271", badgeNumber: "");
			CreatePerson(factory, "MIGUEL A.", "GARZA", externalId: "1047085", badgeNumber: "28412");
			CreatePerson(factory, "SAMUEL", "GARZA", externalId: "1044716", badgeNumber: "");
			CreatePerson(factory, "TONY", "GARZA", externalId: "1046967", badgeNumber: "");
			CreatePerson(factory, "ARMANDO", "GARZA JR", externalId: "1046416", badgeNumber: "");
			CreatePerson(factory, "RUSSELL", "GATES ll", externalId: "1047988", badgeNumber: "28767");
			CreatePerson(factory, "ALEJANDRO", "GAYTAN", externalId: "1035806", badgeNumber: "24008");
			CreatePerson(factory, "GREGARY", "GERMANY", externalId: "1023952", badgeNumber: "10064");
			CreatePerson(factory, "JASON", "GIBSON", externalId: "1042302", badgeNumber: "18662");
			CreatePerson(factory, "FRANCIL", "GILBERT", externalId: "", badgeNumber: "");
			CreatePerson(factory, "BOLIVER", "GOICOCHEA JR.", externalId: "1046932", badgeNumber: "28399");
			CreatePerson(factory, "JERRY", "GOINS", externalId: "1025559", badgeNumber: "");
			CreatePerson(factory, "OBIDIO", "GOMEZ", externalId: "1041527", badgeNumber: "18703");
			CreatePerson(factory, "JOE", "GONZALES", externalId: "1047842", badgeNumber: "");
			CreatePerson(factory, "ARTURO", "GONZALEZ", externalId: "1045651", badgeNumber: "");
			CreatePerson(factory, "EDWARD", "GONZALEZ", externalId: "1045620", badgeNumber: "");
			CreatePerson(factory, "FELIPE", "GONZALEZ", externalId: "1044523", badgeNumber: "");
			CreatePerson(factory, "JESUS", "GONZALEZ", externalId: "1042453", badgeNumber: "24753");
			CreatePerson(factory, "JOSE B.", "GONZALEZ", externalId: "1046933", badgeNumber: "28398");
			CreatePerson(factory, "JOSE G", "GONZALEZ", externalId: "1045921", badgeNumber: "28320");
			CreatePerson(factory, "MANUEL", "GONZALEZ", externalId: "1044506", badgeNumber: "");
			CreatePerson(factory, "PASCUAL", "GONZALEZ", externalId: "1043461", badgeNumber: "28595");
			CreatePerson(factory, "RAFAEL", "GONZALEZ", externalId: "1044073", badgeNumber: "");
			CreatePerson(factory, "ROSENDO", "GONZALEZ", externalId: "1044503", badgeNumber: "");
			CreatePerson(factory, "VEDALINGAM", "GOPALDEVAR", externalId: "1046546", badgeNumber: "28106");
			CreatePerson(factory, "WILLIAM", "GORI", externalId: "1035104", badgeNumber: "14624~28256");
			CreatePerson(factory, "JAMES", "GORMAN", externalId: "1034266", badgeNumber: "18770");
			CreatePerson(factory, "TOMAS", "GORRA", externalId: "1045622", badgeNumber: "");
			CreatePerson(factory, "FERNANDO", "GRACIA", externalId: "1031062", badgeNumber: "");
			CreatePerson(factory, "FRANK", "GRACIA", externalId: "", badgeNumber: "");
			CreatePerson(factory, "FRANK", "GRACIA JR.", externalId: "1037754", badgeNumber: "28792");
			CreatePerson(factory, "KRISTIN", "GRAY", externalId: "1046576", badgeNumber: "28192");
			CreatePerson(factory, "MONIQUE", "GREGORY", externalId: "1044117", badgeNumber: "");
			CreatePerson(factory, "DAVID", "GROVES", externalId: "1044499", badgeNumber: "");
			CreatePerson(factory, "ENRIQUE", "GUERRA", externalId: "", badgeNumber: "");
			CreatePerson(factory, "MICHAEL", "GUERRA", externalId: "1044595", badgeNumber: "");
			CreatePerson(factory, "SCOTTY", "GUILLORY", externalId: "1046911", badgeNumber: "");
			CreatePerson(factory, "JOSE", "GUTIERREZ", externalId: "1044502", badgeNumber: "");
			CreatePerson(factory, "ROBERT", "HACKETT", externalId: "1042814", badgeNumber: "");
			CreatePerson(factory, "KENNETH", "HAIRSTON", externalId: "1046492", badgeNumber: "");
			CreatePerson(factory, "DON", "HALL", externalId: "1025756", badgeNumber: "3792");
			CreatePerson(factory, "KYLE", "HALL", externalId: "1047229", badgeNumber: "");
			CreatePerson(factory, "JAMES", "HAMPTON", externalId: "1045768", badgeNumber: "");
			CreatePerson(factory, "SHERRY", "HARRIS", externalId: "1044693", badgeNumber: "");
			CreatePerson(factory, "CHARLES", "HATTEN", externalId: "1044685", badgeNumber: "");
			CreatePerson(factory, "SPENCER", "HATTEN", externalId: "1044650", badgeNumber: "");
			CreatePerson(factory, "SPENCER", "HATTEN", externalId: "1048044", badgeNumber: "28709");
			CreatePerson(factory, "DERRICK", "HAYNES", externalId: "1043407", badgeNumber: "24667~24788~28070~28721");
			CreatePerson(factory, "RANDI", "HAYNES", externalId: "1047031", badgeNumber: "");
			CreatePerson(factory, "JAMES", "HEBERT", externalId: "1046685", badgeNumber: "");
			CreatePerson(factory, "ALBERTO", "HERNANDEZ", externalId: "1041530", badgeNumber: "");
			CreatePerson(factory, "EDUARDO", "HERNANDEZ", externalId: "1044691", badgeNumber: "24852");
			CreatePerson(factory, "JASON", "HERNANDEZ", externalId: "1044425", badgeNumber: "24585~24630");
			CreatePerson(factory, "JOSE", "HERNANDEZ", externalId: "1045715", badgeNumber: "");
			CreatePerson(factory, "JUAN", "HERNANDEZ", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JUAN", "HERNANDEZ", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JOSE", "HERNANDEZ JR.", externalId: "1045782", badgeNumber: "");
			CreatePerson(factory, "MATHEW", "HERNANDEZ, JR.", externalId: "1046734", badgeNumber: "");
			CreatePerson(factory, "SERGIO", "HERRERA", externalId: "1046381", badgeNumber: "28109~28586");
			CreatePerson(factory, "TETRIN", "HILL", externalId: "1046573", badgeNumber: "");
			CreatePerson(factory, "RUSSELL", "HIMBURG", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JOHN", "HOBBS", externalId: "1031165", badgeNumber: "24682");
			CreatePerson(factory, "STEPHEN", "HOERNER", externalId: "1041980", badgeNumber: "23987");
			CreatePerson(factory, "LARRY", "HOFFMAN", externalId: "1043270", badgeNumber: "28112");
			CreatePerson(factory, "TERESA", "HOLLANDSWORTH", externalId: "1044551", badgeNumber: "24582");
			CreatePerson(factory, "JOSHUA", "HOLLEN", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JORDAN", "HOLMAN", externalId: "1038013", badgeNumber: "");
			CreatePerson(factory, "BRETT", "HOOPER", externalId: "1047269", badgeNumber: "");
			CreatePerson(factory, "DAVID", "HOPE", externalId: "1047595", badgeNumber: "28444");
			CreatePerson(factory, "JAMEY", "HRABIK", externalId: "1046373", badgeNumber: "28111");
			CreatePerson(factory, "RYAN", "HUDMAN", externalId: "1046937", badgeNumber: "28319");
			CreatePerson(factory, "JOSE", "HUERTA", externalId: "1044637", badgeNumber: "");
			CreatePerson(factory, "ROBERTO", "HUERTA", externalId: "1047097", badgeNumber: "");
			CreatePerson(factory, "ALEJANDRO", "IBARRA", externalId: "", badgeNumber: "");
			CreatePerson(factory, "GUMER", "IBARRA", externalId: "1044522", badgeNumber: "");
			CreatePerson(factory, "ISRAEL", "IBARRA", externalId: "1047276", badgeNumber: "");
			CreatePerson(factory, "JUAN", "IBARRA", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JUAN", "IBARRA", externalId: "1046769", badgeNumber: "28263");
			CreatePerson(factory, "MIGUEL", "INFANTE", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JACKIE", "INMAN", externalId: "1036090", badgeNumber: "");
			CreatePerson(factory, "JONATHAN", "ISAAC", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JONATHAN", "ISSAC", externalId: "", badgeNumber: "");
			CreatePerson(factory, "MARTY", "JAMISON", externalId: "1046377", badgeNumber: "24542");
			CreatePerson(factory, "GUSTAVO", "JARAMILLO", externalId: "1047479", badgeNumber: "28360");
			CreatePerson(factory, "RAFAEL", "JARAMILLO", externalId: "1043629", badgeNumber: "23946");
			CreatePerson(factory, "MATTHEW", "JARES", externalId: "1044682", badgeNumber: "24674");
			CreatePerson(factory, "ALBERT", "JENKINS", externalId: "1044504", badgeNumber: "");
			CreatePerson(factory, "ROY", "JENKINS", externalId: "1044154", badgeNumber: "24128");
			CreatePerson(factory, "JEREMIAH", "JIMENEZ", externalId: "1047165", badgeNumber: "28536");
			CreatePerson(factory, "JESUS", "JIMENEZ", externalId: "1046380", badgeNumber: "");
			CreatePerson(factory, "JOSE", "JIMENEZ", externalId: "1039747", badgeNumber: "24615");
			CreatePerson(factory, "MIGUEL", "JIMENEZ", externalId: "1039339", badgeNumber: "");
			CreatePerson(factory, "ROSENDO", "JIMENEZ", externalId: "1038377", badgeNumber: "24649~27905");
			CreatePerson(factory, "SATURNINO", "JIMENEZ", externalId: "1026098", badgeNumber: "14688");
			CreatePerson(factory, "ANTHONY", "JOHNSON", externalId: "1046445", badgeNumber: "");
			CreatePerson(factory, "CHARLES", "JOHNSON", externalId: "1047864", badgeNumber: "28724");
			CreatePerson(factory, "JAMES", "JOHNSON", externalId: "1044178", badgeNumber: "24143");
			CreatePerson(factory, "JAMES", "JOHNSON", externalId: "1044734", badgeNumber: "");
			CreatePerson(factory, "TERRY", "JOHNSON", externalId: "1044714", badgeNumber: "24294");
			CreatePerson(factory, "WILLARD", "JOHNSON", externalId: "1045594", badgeNumber: "18320~28702");
			CreatePerson(factory, "JASON", "JONES", externalId: "1044116", badgeNumber: "");
			CreatePerson(factory, "JOHN", "JONES", externalId: "1044510", badgeNumber: "");
			CreatePerson(factory, "SHELBY", "JONES", externalId: "1046594", badgeNumber: "");
			CreatePerson(factory, "MARTIN", "JUAREZ", externalId: "1042161", badgeNumber: "18092");
			CreatePerson(factory, "DANIEL", "JUERGENS", externalId: "1046419", badgeNumber: "");
			CreatePerson(factory, "RANDY", "KADURA", externalId: "1043082", badgeNumber: "28580");
			CreatePerson(factory, "WALTER", "KALISZ", externalId: "1042308", badgeNumber: "24790");
			CreatePerson(factory, "ISIAH", "KELLUP", externalId: "1044684", badgeNumber: "24675");
			CreatePerson(factory, "WALTER", "KINCAID", externalId: "1031169", badgeNumber: "24441~18853");
			CreatePerson(factory, "BROCK", "KING", externalId: "1044672", badgeNumber: "");
			CreatePerson(factory, "DANE", "KISSOONDATH", externalId: "1045593", badgeNumber: "24820");
			CreatePerson(factory, "RICHARD", "KOVALCIK", externalId: "1047482", badgeNumber: "28441");
			CreatePerson(factory, "JESUS", "LAINEZ", externalId: "1047146", badgeNumber: "");
			CreatePerson(factory, "ERIC", "LANNING", externalId: "1041500", badgeNumber: "24207~18636");
			CreatePerson(factory, "ALONZO", "LEAL", externalId: "1042377", badgeNumber: "");
			CreatePerson(factory, "GILBERTO", "LEAL", externalId: "1042304", badgeNumber: "");
			CreatePerson(factory, "PAUL", "LeBLANC", externalId: "1047148", badgeNumber: "28464");
			CreatePerson(factory, "BRANDON", "LEBLANC", externalId: "", badgeNumber: "28147");
			CreatePerson(factory, "BRANDON", "LEBLANC", externalId: "1047238", badgeNumber: "28770");
			CreatePerson(factory, "STEVEN", "LECINSKI", externalId: "1043450", badgeNumber: "");
			CreatePerson(factory, "JACOB", "LEE", externalId: "1042788", badgeNumber: "18734");
			CreatePerson(factory, "JOSEPH", "LEWIS", externalId: "1026076", badgeNumber: "28470");
			CreatePerson(factory, "RANDY", "LINDSTORM", externalId: "1041501", badgeNumber: "24455~18669");
			CreatePerson(factory, "JOSE A.", "LION JR.", externalId: "1047564", badgeNumber: "28550");
			CreatePerson(factory, "THOMAS", "LISANO", externalId: "1037254", badgeNumber: "");
			CreatePerson(factory, "VIRGIL", "LITTLE", externalId: "1038981", badgeNumber: "");
			CreatePerson(factory, "RUBEN", "LOERA", externalId: "", badgeNumber: "");
			CreatePerson(factory, "RUBEN", "LOERA", externalId: "", badgeNumber: "");
			CreatePerson(factory, "JONATHAN", "LONG", externalId: "1030534", badgeNumber: "28099");
			CreatePerson(factory, "JOHNNY L.", "LOPEZ", externalId: "1047565", badgeNumber: "");
			CreatePerson(factory, "JOSE", "LOPEZ", externalId: "1031031", badgeNumber: "23933");
			CreatePerson(factory, "JOSE", "LOPEZ", externalId: "1027947", badgeNumber: "18159");
			CreatePerson(factory, "OSCAR", "LOPEZ", externalId: "1036891", badgeNumber: "10522");
			CreatePerson(factory, "RAY", "LOPEZ", externalId: "1020558", badgeNumber: "28305~00707");
			CreatePerson(factory, "DARREN", "LOVILLE", externalId: "1047597", badgeNumber: "28520");
			CreatePerson(factory, "ALFONSO", "LOZANO", externalId: "1046912", badgeNumber: "28382");
			CreatePerson(factory, "JUAN", "LUCIO", externalId: "1040055", badgeNumber: "");
			CreatePerson(factory, "JOE", "LUGO", externalId: "1022889", badgeNumber: "23955");
			CreatePerson(factory, "JOSUE", "LUNA", externalId: "1046999", badgeNumber: "");
			CreatePerson(factory, "CHRISTOPHER", "MACHE", externalId: "", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "MACIAS", externalId: "1027684", badgeNumber: "3605");
			CreatePerson(factory, "DAVID", "MACIAS", externalId: "1044442", badgeNumber: "");
			CreatePerson(factory, "PEDRO", "MAGALLANES", externalId: "1044638", badgeNumber: "");
			CreatePerson(factory, "LUIS", "MALDONADO", externalId: "1043982", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "MARQUEZ", externalId: "", badgeNumber: "");
			CreatePerson(factory, "PHILLIP", "MARRON", externalId: "1044721", badgeNumber: "24281");
			CreatePerson(factory, "JERRY", "MARTIN", externalId: "1040123", badgeNumber: "18807~24246");
			CreatePerson(factory, "RYAN", "MARTIN", externalId: "1046414", badgeNumber: "");
			CreatePerson(factory, "ABIEL", "MARTINEZ", externalId: "1044712", badgeNumber: "");
			CreatePerson(factory, "ARTHUR", "MARTINEZ", externalId: "1046806", badgeNumber: "");
			CreatePerson(factory, "JOSE", "MARTINEZ", externalId: "1046819", badgeNumber: "28293");
			CreatePerson(factory, "JOSE LUIS", "MARTINEZ", externalId: "1044519", badgeNumber: "28318");
			CreatePerson(factory, "JUAN", "MARTINEZ", externalId: "1033471", badgeNumber: "27935");
			CreatePerson(factory, "MAURICIO", "MARTINEZ", externalId: "1046966", badgeNumber: "28310");
			CreatePerson(factory, "RAFAEL", "MARTINEZ", externalId: "1045443", badgeNumber: "24339");
			CreatePerson(factory, "RICARDO", "MARTINEZ", externalId: "1039336", badgeNumber: "14500");
			CreatePerson(factory, "CHRIS", "MASON", externalId: "1047483", badgeNumber: "28560");
			CreatePerson(factory, "BRIAN", "MATLOCK", externalId: "1038941", badgeNumber: "14547");
			CreatePerson(factory, "STEVE", "MAZZOLA", externalId: "1045441", badgeNumber: "");
			CreatePerson(factory, "WALTON", "MCCORD", externalId: "1044150", badgeNumber: "24167");
			CreatePerson(factory, "EDWARD", "MCDANIEL", externalId: "1035208", badgeNumber: "3965");
			CreatePerson(factory, "GARLAND", "MCFARLAND", externalId: "1048033", badgeNumber: "28776");
			CreatePerson(factory, "ROBERT", "MCMILLAN", externalId: "1025677", badgeNumber: "14515");
			CreatePerson(factory, "ROBERT A", "MCMILLAN", externalId: "1026097", badgeNumber: "24789");
			CreatePerson(factory, "NATHAN", "MCWHORTER", externalId: "1042022", badgeNumber: "");
			CreatePerson(factory, "ARTURO", "MEARS", externalId: "1047265", badgeNumber: "");
			CreatePerson(factory, "JUAN", "MEDINA", externalId: "1046735", badgeNumber: "");
			CreatePerson(factory, "MANUEL", "MEDINA", externalId: "1046347", badgeNumber: "28032");
			CreatePerson(factory, "ARLES", "MEJIA", externalId: "1037979", badgeNumber: "23995");
			CreatePerson(factory, "JUAN", "MENDEZ", externalId: "1045741", badgeNumber: "28131");
			CreatePerson(factory, "ROLANDO", "MENDOZA", externalId: "1047048", badgeNumber: "");
			CreatePerson(factory, "BILL", "MERCER", externalId: "1030900", badgeNumber: "");
			CreatePerson(factory, "JERRY", "MERCHANT", externalId: "1020395", badgeNumber: "24192~18490");
			CreatePerson(factory, "PAUL", "MERRELL", externalId: "1025068", badgeNumber: "03199~28054");
			CreatePerson(factory, "BOBBY", "MITCHELL", externalId: "1043201", badgeNumber: "");
			CreatePerson(factory, "MARK", "MITCHELL", externalId: "1048038", badgeNumber: "28711");
			CreatePerson(factory, "FERNANDO", "MONTEMAYOR", externalId: "1046893", badgeNumber: "28307");
			CreatePerson(factory, "OMAR", "MONTEMAYOR JR", externalId: "1047275", badgeNumber: "");
			CreatePerson(factory, "JOEL", "MONTES JR.", externalId: "1047719", badgeNumber: "");
			CreatePerson(factory, "KENNETH", "MOON", externalId: "1042156", badgeNumber: "24507");
			CreatePerson(factory, "JUAN", "MORA", externalId: "1045345", badgeNumber: "");
			CreatePerson(factory, "ALAIN", "MORALES", externalId: "1035549", badgeNumber: "");
			CreatePerson(factory, "ANDY", "MORALES", externalId: "1043996", badgeNumber: "24783");
			CreatePerson(factory, "JOSHUA", "MORTON", externalId: "1047100", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "MORUA", externalId: "1044431", badgeNumber: "");
			CreatePerson(factory, "RILEY", "MOSS", externalId: "1039375", badgeNumber: "");
			CreatePerson(factory, "KENNETH", "MOYE", externalId: "1020397", badgeNumber: "24623~23978~28281");
			CreatePerson(factory, "DENNIS", "MULLINS", externalId: "1037530", badgeNumber: "10883");
			CreatePerson(factory, "DIANA", "MUNOZ", externalId: "1035014", badgeNumber: "14430");
			CreatePerson(factory, "JOSE", "MUNOZ", externalId: "1030952", badgeNumber: "1660");
			CreatePerson(factory, "SHAJI", "NANOO", externalId: "", badgeNumber: "");
			CreatePerson(factory, "PEDRO", "NEGRETE", externalId: "1044153", badgeNumber: "24225~24193");
			CreatePerson(factory, "MATHEW", "NICHOLS", externalId: "1047164", badgeNumber: "28459");
			CreatePerson(factory, "JULIO", "NIETO", externalId: "1020480", badgeNumber: "2213");
			CreatePerson(factory, "LOUIS", "NOBLE JR.", externalId: "1039308", badgeNumber: "28178");
			CreatePerson(factory, "SLY", "NORMAN", externalId: "1036231", badgeNumber: "24251");
			CreatePerson(factory, "EDUARDO", "OCHOA", externalId: "1032954", badgeNumber: "10875");
			CreatePerson(factory, "LEONARD", "OCTAVE", externalId: "1046776", badgeNumber: "");
			CreatePerson(factory, "FLOYD", "ODIE", externalId: "1046728", badgeNumber: "");
			CreatePerson(factory, "EDGAR", "OLIVO", externalId: "1047849", badgeNumber: "28728");
			CreatePerson(factory, "LUIS", "ONTIVEROS", externalId: "1046379", badgeNumber: "");
			CreatePerson(factory, "DAVID", "ORTIZ", externalId: "1026217", badgeNumber: "2136");
			CreatePerson(factory, "JORGE", "OSARIO", externalId: "1047230", badgeNumber: "");
			CreatePerson(factory, "JEFFREY", "OTEMS", externalId: "1042752", badgeNumber: "24064");
			CreatePerson(factory, "LOUESHION", "OWENS", externalId: "1035977", badgeNumber: "28583");
			CreatePerson(factory, "MICHAEL", "OWENS", externalId: "1044634", badgeNumber: "");
			CreatePerson(factory, "LUIS", "PACHE", externalId: "1032952", badgeNumber: "28204");
			CreatePerson(factory, "JUAN", "PALENCIA-PEREZ", externalId: "1048010", badgeNumber: "28703");
			CreatePerson(factory, "COLBY", "PANNELL", externalId: "1045781", badgeNumber: "28471");
			CreatePerson(factory, "FRANCISCO", "PAREDES", externalId: "1046727", badgeNumber: "");
			CreatePerson(factory, "EUGENIO", "PECINA", externalId: "1022572", badgeNumber: "28041");
			CreatePerson(factory, "JOSE", "PEDRAZA", externalId: "1047267", badgeNumber: "");
			CreatePerson(factory, "RODRIGO", "PENA", externalId: "1037746", badgeNumber: "10818~28021");
			CreatePerson(factory, "ANA", "PERALTA", externalId: "1044692", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "PEREZ", externalId: "1042701", badgeNumber: "24040");
			CreatePerson(factory, "CARLOS", "PEREZ", externalId: "1043892", badgeNumber: "24749");
			CreatePerson(factory, "NATHAN", "PEREZ", externalId: "1043965", badgeNumber: "24812");
			CreatePerson(factory, "SERGIO", "PEREZ-DURAN", externalId: "1047115", badgeNumber: "28774");
			CreatePerson(factory, "MURPHY", "PETROVANI", externalId: "1047897", badgeNumber: "28748");
			CreatePerson(factory, "MICHAEL", "PETTERSON", externalId: "1047697", badgeNumber: "");
			CreatePerson(factory, "RUDOLPH", "PHILLIP", externalId: "1046572", badgeNumber: "");
			CreatePerson(factory, "MICHAEL", "PIZARRO", externalId: "1047063", badgeNumber: "28369");
			CreatePerson(factory, "RONALD", "PLAMANN", externalId: "", badgeNumber: "27997");
			CreatePerson(factory, "RONALD", "PLAMANN", externalId: "", badgeNumber: "");
			CreatePerson(factory, "RONALD", "PLAMANN", externalId: "1047239", badgeNumber: "28502");
			CreatePerson(factory, "MANITHOMAS", "PLAPPARAMBIL", externalId: "1047272", badgeNumber: "");
			CreatePerson(factory, "JASON", "POSTEL", externalId: "1033623", badgeNumber: "");
			CreatePerson(factory, "HAL", "POUNDS", externalId: "1026187", badgeNumber: "1766");
			CreatePerson(factory, "LUIS", "PRADO", externalId: "41152", badgeNumber: "");
			CreatePerson(factory, "THEODORE", "PREGEANT", externalId: "1047068", badgeNumber: "28353");
			CreatePerson(factory, "GREGORY", "PRIEST", externalId: "1047989", badgeNumber: "28700");
			CreatePerson(factory, "MIGUEL", "PULIDO", externalId: "1042373", badgeNumber: "24836");
			CreatePerson(factory, "KORAH", "PUNNATHARA", externalId: "1046731", badgeNumber: "");
			CreatePerson(factory, "RICARDO", "QUINTERO", externalId: "1036459", badgeNumber: "14398~28182");
			CreatePerson(factory, "BENJAMIN", "RAMIREZ", externalId: "1031902", badgeNumber: "28575");
			CreatePerson(factory, "BERTHA", "RAMIREZ", externalId: "", badgeNumber: "28499");
			CreatePerson(factory, "JUAN", "RAMIREZ", externalId: "1035804", badgeNumber: "24716");
			CreatePerson(factory, "MARTIN", "RAMIREZ", externalId: "1047481", badgeNumber: "28150");
			CreatePerson(factory, "MIGUEL", "RAMIREZ", externalId: "1046771", badgeNumber: "28708");
			CreatePerson(factory, "DEONE", "RAMSEY", externalId: "1045629", badgeNumber: "");
			CreatePerson(factory, "GENARO", "RANGEL", externalId: "1046444", badgeNumber: "");
			CreatePerson(factory, "JAMES", "REESE", externalId: "1048080", badgeNumber: "28799");
			CreatePerson(factory, "JOSEPH", "REEVES", externalId: "1047020", badgeNumber: "");
			CreatePerson(factory, "GABRIEL", "REYES", externalId: "1046342", badgeNumber: "28247");
			CreatePerson(factory, "LARRY", "REYES", externalId: "1026195", badgeNumber: "");
			CreatePerson(factory, "ROLANDO", "REYES", externalId: "1044436", badgeNumber: "");
			CreatePerson(factory, "WILMER", "REYES", externalId: "1045780", badgeNumber: "28468");
			CreatePerson(factory, "JASON", "RICHARD", externalId: "1047098", badgeNumber: "28370");
			CreatePerson(factory, "CORY", "RITCH", externalId: "1045769", badgeNumber: "");
			CreatePerson(factory, "JORGE", "RIVERA", externalId: "1048032", badgeNumber: "28729");
			CreatePerson(factory, "JUAN RICARDO", "RIVERA", externalId: "1045643", badgeNumber: "18317");
			CreatePerson(factory, "HOWARD", "ROBERTSON", externalId: "1020418", badgeNumber: "641");
			CreatePerson(factory, "COREY", "ROBINS", externalId: "1047827", badgeNumber: "28736");
			CreatePerson(factory, "PEDRO", "ROBLES", externalId: "1045621", badgeNumber: "24846");
			CreatePerson(factory, "CHRIS", "RODGER", externalId: "1046220", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "RODRIGUEZ", externalId: "1046736", badgeNumber: "");
			CreatePerson(factory, "EDUARDO", "RODRIGUEZ", externalId: "1020564", badgeNumber: "14127");
			CreatePerson(factory, "FIDENCIO", "RODRIGUEZ", externalId: "1044151", badgeNumber: "24166");
			CreatePerson(factory, "JOSE", "RODRIGUEZ", externalId: "1046273", badgeNumber: "28001");
			CreatePerson(factory, "JOSE", "RODRIGUEZ", externalId: "1041350", badgeNumber: "18001");
			CreatePerson(factory, "JOSE", "RODRIGUEZ", externalId: "1041528", badgeNumber: "24469");
			CreatePerson(factory, "JUAN", "RODRIGUEZ", externalId: "1043488", badgeNumber: "23997");
			CreatePerson(factory, "MARLA", "RODRIGUEZ", externalId: "1047000", badgeNumber: "28414");
			CreatePerson(factory, "OSCAR", "RODRIGUEZ", externalId: "1047999", badgeNumber: "28701");
			CreatePerson(factory, "REYNALDO", "RODRIGUEZ", externalId: "1043345", badgeNumber: "");
			CreatePerson(factory, "RICARDO", "RODRIGUEZ", externalId: "", badgeNumber: "");
			CreatePerson(factory, "ERNEST", "ROMAN", externalId: "1040097", badgeNumber: "14633");
			CreatePerson(factory, "JASON", "ROMAN", externalId: "1038436", badgeNumber: "14246");
			CreatePerson(factory, "ROBERTO", "ROMERO", externalId: "1044118", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "ROSALES", externalId: "1024016", badgeNumber: "24565~00831");
			CreatePerson(factory, "GERARDO", "ROSSEL", externalId: "1044666", badgeNumber: "");
			CreatePerson(factory, "EUGENE", "ROUSSELL", externalId: "1026748", badgeNumber: "28358");
			CreatePerson(factory, "KENNETH", "RUDDICK", externalId: "1047250", badgeNumber: "");
			CreatePerson(factory, "LUIS", "RUEDA", externalId: "1047026", badgeNumber: "28274");
			CreatePerson(factory, "MARC", "RUSSELL", externalId: "1046415", badgeNumber: "28040");
			CreatePerson(factory, "JUAN", "SALAZAR", externalId: "1043964", badgeNumber: "28130");
			CreatePerson(factory, "MIKE", "SALAZAR", externalId: "1046684", badgeNumber: "28218");
			CreatePerson(factory, "GERARDO", "SALAZAR - ALVAREZ", externalId: "1046737", badgeNumber: "28790");
			CreatePerson(factory, "JOSE", "SALGUERO", externalId: "1040022", badgeNumber: "17964");
			CreatePerson(factory, "ARGELIO", "SALINAS", externalId: "1042822", badgeNumber: "18759~24622");
			CreatePerson(factory, "HUMBERTO", "SALINAS", externalId: "1045623", badgeNumber: "24814");
			CreatePerson(factory, "OZIEL", "SALINAS", externalId: "1045646", badgeNumber: "27901");
			CreatePerson(factory, "ROGELIO", "SALINAS", externalId: "1020419", badgeNumber: "14445");
			CreatePerson(factory, "JOSE", "SAMUDO", externalId: "1047050", badgeNumber: "");
			CreatePerson(factory, "RICARDO", "SANCHEZ", externalId: "1047032", badgeNumber: "28376");
			CreatePerson(factory, "GENE", "SANDERS", externalId: "1046636", badgeNumber: "28175");
			CreatePerson(factory, "PERRY GENE", "SANDERS", externalId: "", badgeNumber: "");
			CreatePerson(factory, "FRANCISCO", "SANDOVAL", externalId: "1046869", badgeNumber: "28330");
			CreatePerson(factory, "NOEL", "SANDOVAL", externalId: "1043273", badgeNumber: "");
			CreatePerson(factory, "PONCIANO", "SANMIGUEL", externalId: "1035602", badgeNumber: "9905");
			CreatePerson(factory, "PONCIANO", "SAN MIGUEL lll", externalId: "1046675", badgeNumber: "28214");
			CreatePerson(factory, "FRANCISCO", "SAUCEDO", externalId: "1044428", badgeNumber: "");
			CreatePerson(factory, "JOSHUA", "SCHMIDT", externalId: "1047958", badgeNumber: "28762");
			CreatePerson(factory, "OLTON", "SCOTT JR.", externalId: "1047209", badgeNumber: "28455");
			CreatePerson(factory, "JOSE", "SEGOVIA", externalId: "1039345", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "SIERRA", externalId: "1036804", badgeNumber: "28101");
			CreatePerson(factory, "SANDRA", "SIERRA", externalId: "1045964", badgeNumber: "24849");
			CreatePerson(factory, "LUCIO", "SILVA", externalId: "1047270", badgeNumber: "");
			CreatePerson(factory, "ROBERT", "SILVAS", externalId: "1047863", badgeNumber: "");
			CreatePerson(factory, "THOMAS", "SLOAN", externalId: "", badgeNumber: "");
			CreatePerson(factory, "THOMAS", "SLOANE", externalId: "1047478", badgeNumber: "");
			CreatePerson(factory, "CARYOLYN", "SMITH-MCMAHAN", externalId: "1046913", badgeNumber: "28383");
			CreatePerson(factory, "CRISTINO", "SOLIZ", externalId: "1045445", badgeNumber: "");
			CreatePerson(factory, "BERTHA", "SOSA", externalId: "", badgeNumber: "28363");
			CreatePerson(factory, "CRISTINO", "SOSA", externalId: "1043967", badgeNumber: "24078");
			CreatePerson(factory, "ALEX", "ST. ANGE", externalId: "1022000", badgeNumber: "10825");
			CreatePerson(factory, "JUSTIN", "STANLEY", externalId: "1045966", badgeNumber: "28719");
			CreatePerson(factory, "SAM", "STANLEY", externalId: "1046714", badgeNumber: "");
			CreatePerson(factory, "GREGORY", "ST MARTIN", externalId: "1039588", badgeNumber: "18191");
			CreatePerson(factory, "ROBERT", "STOKER", externalId: "1045625", badgeNumber: "");
			CreatePerson(factory, "BRYCE", "STONE", externalId: "1044075", badgeNumber: "");
			CreatePerson(factory, "GREG", "SUJO", externalId: "", badgeNumber: "");
			CreatePerson(factory, "GREG", "SUJO", externalId: "", badgeNumber: "");
			CreatePerson(factory, "BERNIE", "SULLIVAN", externalId: "1025707", badgeNumber: "18598");
			CreatePerson(factory, "KEVIN", "SWEEZY", externalId: "", badgeNumber: "28545");
			CreatePerson(factory, "DAVID", "SWETT", externalId: "1042371", badgeNumber: "24439");
			CreatePerson(factory, "THOMAS", "TACKETT", externalId: "1046964", badgeNumber: "28314");
			CreatePerson(factory, "MARCUIS", "TAYLOR", externalId: "1043025", badgeNumber: "18676");
			CreatePerson(factory, "MICHAEL", "TAYLOR", externalId: "1036805", badgeNumber: "");
			CreatePerson(factory, "WILLIAM", "THIBODEAUX", externalId: "1045812", badgeNumber: "27978");
			CreatePerson(factory, "HORACE", "THOMAS", externalId: "1035480", badgeNumber: "");
			CreatePerson(factory, "MARK", "THOMPSON", externalId: "1039982", badgeNumber: "");
			CreatePerson(factory, "HECTOR", "TIJE", externalId: "1045767", badgeNumber: "");
			CreatePerson(factory, "MICHAEL", "TIJERINA", externalId: "1026241", badgeNumber: "");
			CreatePerson(factory, "DAVID W", "TIPTON", externalId: "1046384", badgeNumber: "");
			CreatePerson(factory, "BENJAMIN", "TORRES", externalId: "1044505", badgeNumber: "");
			CreatePerson(factory, "EDUARDO", "TORRES", externalId: "1046353", badgeNumber: "");
			CreatePerson(factory, "ERIC", "TORRES", externalId: "1044498", badgeNumber: "24523");
			CreatePerson(factory, "JOE", "TORRES", externalId: "1024824", badgeNumber: "");
			CreatePerson(factory, "RAFAEL", "TORRES", externalId: "1043970", badgeNumber: "24083~28292");
			CreatePerson(factory, "FRANCISCO", "TREJO", externalId: "1044683", badgeNumber: "");
			CreatePerson(factory, "NICK", "TREVINO", externalId: "1037436", badgeNumber: "28317~24847");
			CreatePerson(factory, "JOSE", "TRUJILLO", externalId: "1031931", badgeNumber: "");
			CreatePerson(factory, "JESUS", "TUDON", externalId: "1047233", badgeNumber: "");
			CreatePerson(factory, "CHARLES", "TURNER", externalId: "1047133", badgeNumber: "");
			CreatePerson(factory, "DAVID", "TURNER", externalId: "1020430", badgeNumber: "28758");
			CreatePerson(factory, "MARCUS", "TURNER", externalId: "1046293", badgeNumber: "28089~28180");
			CreatePerson(factory, "HUNTER", "UNDERWOOD", externalId: "1047720", badgeNumber: "");
			CreatePerson(factory, "EDWARD", "VALENCIA", externalId: "1047828", badgeNumber: "28746");
			CreatePerson(factory, "PEDRO", "VALENCIA", externalId: "1047119", badgeNumber: "");
			CreatePerson(factory, "MARIANO", "VALERIO", externalId: "1044570", badgeNumber: "");
			CreatePerson(factory, "JOSE", "VASQUEZ", externalId: "1046820", badgeNumber: "28186~28793");
			CreatePerson(factory, "JUAN", "VASQUEZ", externalId: "1035678", badgeNumber: "");
			CreatePerson(factory, "JUAN RENE", "VASQUEZ", externalId: "1047658", badgeNumber: "28519");
			CreatePerson(factory, "ROGER", "VASQUEZ", externalId: "1045784", badgeNumber: "");
			CreatePerson(factory, "CHRISTIAN", "VAZQUEZ", externalId: "1046388", badgeNumber: "");
			CreatePerson(factory, "DANIEL", "VEGA", externalId: "1046472", badgeNumber: "28105");
			CreatePerson(factory, "SILVERIO", "VELASQUEZ", externalId: "1034828", badgeNumber: "24687");
			CreatePerson(factory, "CONCEPCION", "VELAZQUEZ", externalId: "1046274", badgeNumber: "28091");
			CreatePerson(factory, "JONATHAN", "VELAZQUEZ", externalId: "1047019", badgeNumber: "");
			CreatePerson(factory, "SALOMON", "VELAZQUEZ", externalId: "1047001", badgeNumber: "28308");
			CreatePerson(factory, "KEVIN", "VICK", externalId: "1045922", badgeNumber: "27912");
			CreatePerson(factory, "MIGUEL", "VILLALON III", externalId: "", badgeNumber: "28472");
			CreatePerson(factory, "EDWIN", "VILLALTA", externalId: "1044700", badgeNumber: "");
			CreatePerson(factory, "MARTIN", "VILLANUEVA JR.", externalId: "1046595", badgeNumber: "28179");
			CreatePerson(factory, "MARIO", "VILLARREAL", externalId: "1043319", badgeNumber: "18856");
			CreatePerson(factory, "SERGIO", "VILLARREAL", externalId: "1044782", badgeNumber: "");
			CreatePerson(factory, "VICTOR", "VILLARREAL II", externalId: "1047524", badgeNumber: "28596");
			CreatePerson(factory, "ARMANDO", "VILLEDA", externalId: "1046866", badgeNumber: "28392");
			CreatePerson(factory, "JOHN L.", "VILLERY", externalId: "1047065", badgeNumber: "28493");
			CreatePerson(factory, "JAIME", "WADEL", externalId: "1047945", badgeNumber: "28768");
			CreatePerson(factory, "CLIFF", "WAGLEY", externalId: "1044521", badgeNumber: "");
			CreatePerson(factory, "JORJA", "WAIDE", externalId: "1034326", badgeNumber: "3665");
			CreatePerson(factory, "ERIC", "WALKER", externalId: "1020570", badgeNumber: "");
			CreatePerson(factory, "ERICK", "WARNER", externalId: "1047047", badgeNumber: "28362");
			CreatePerson(factory, "TERRY", "WEATHERSPOON", externalId: "1046386", badgeNumber: "28007");
			CreatePerson(factory, "ANDREW", "WEBB", externalId: "1037809", badgeNumber: "");
			CreatePerson(factory, "CHRIS", "WEBB", externalId: "1046246", badgeNumber: "");
			CreatePerson(factory, "CHRISTOPHER", "WEBB", externalId: "1046716", badgeNumber: "");
			CreatePerson(factory, "RYAN", "WESTMORELAND", externalId: "1043539", badgeNumber: "23915");
			CreatePerson(factory, "STAN", "WHEELER", externalId: "1037417", badgeNumber: "10500");
			CreatePerson(factory, "MORRISON", "WILLETTS", externalId: "1044107", badgeNumber: "24841");
			CreatePerson(factory, "ALLAN", "WILLIAMS", externalId: "1047029", badgeNumber: "28349");
			CreatePerson(factory, "ROY", "WILLIAMS", externalId: "1024360", badgeNumber: "14686");
			CreatePerson(factory, "CHASE", "WILSON", externalId: "1044443", badgeNumber: "24543");
			CreatePerson(factory, "MARK", "WILSON", externalId: "1044720", badgeNumber: "");
			CreatePerson(factory, "TERRY", "WILSON", externalId: "1042319", badgeNumber: "24123~18129");
			CreatePerson(factory, "GLEN", "WOODARD", externalId: "1027048", badgeNumber: "2237");
			CreatePerson(factory, "JAMES", "WOODWARD JR", externalId: "1047896", badgeNumber: "28796");
			CreatePerson(factory, "DAVID", "WOREK", externalId: "1046664", badgeNumber: "28171");
			CreatePerson(factory, "NORMIE", "WRIGHT", externalId: "1020437", badgeNumber: "24178");
			CreatePerson(factory, "ROBERT", "WRIGHT", externalId: "1047898", badgeNumber: "28777");
			CreatePerson(factory, "VICTOR", "YANES", externalId: "1047264", badgeNumber: "");
			CreatePerson(factory, "JEREMY", "YARBROUGH", externalId: "1044227", badgeNumber: "");
			CreatePerson(factory, "MICHELLE", "YARBROUGH", externalId: "1042645", badgeNumber: "");
			CreatePerson(factory, "RALPH", "YATES", externalId: "1028008", badgeNumber: "1541");
			CreatePerson(factory, "DERRICK", "YAWN", externalId: "1037013", badgeNumber: "10682");
			CreatePerson(factory, "ERIC", "YBARRA", externalId: "1047986", badgeNumber: "28734");
			CreatePerson(factory, "CURTIS", "YOUNG", externalId: "1045647", badgeNumber: "24861");
			CreatePerson(factory, "KEVIN", "YOUNG", externalId: "1047277", badgeNumber: "28335");
			CreatePerson(factory, "ALBERT", "ZAMARIPAS", externalId: "1044244", badgeNumber: "");
			CreatePerson(factory, "ARTURO", "ZAMORA", externalId: "1044152", badgeNumber: "24199");
			CreatePerson(factory, "JOE", "ZAMORA", externalId: "1041776", badgeNumber: "17948~27993");
			CreatePerson(factory, "JAIME", "ZAPATA", externalId: "1047088", badgeNumber: "28402");
			CreatePerson(factory, "JOHN", "ZARUBA", externalId: "1044077", badgeNumber: "24744");
			CreatePerson(factory, "ANGEL", "ZEPEDA", externalId: "1042679", badgeNumber: "24831");
			CreatePerson(factory, "CRISTIAN", "ZEPEDA", externalId: "1046584", badgeNumber: "");
			CreatePerson(factory, "RODOLFO", "ZEPEDA", externalId: "1046585", badgeNumber: "");
			CreatePerson(factory, "CARLOS", "ZUBIATE", externalId: "1048042", badgeNumber: "28772");

			DisplayStatus("People staging completed", false);
		}

		private void StageLocationsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Location Stage", true);

			var factory = new Factory(Context);

			var deerPark = factory.createLocation("Deer Park", action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Location, "Deer Park", S2.Id, deerPark.LocationID);
			factory.createExternalApplicationKey(EntityType.Location, "1", Track.Id, deerPark.LocationID);

			DisplayStatus("Added Location: Deer Park", false);

			var bayport = factory.createLocation("Bayport", action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Location, "Bayport", S2.Id, bayport.LocationID);
			factory.createExternalApplicationKey(EntityType.Location, "2", Track.Id, bayport.LocationID);

			DisplayStatus("Added Location: Bayport", false);

			DisplayStatus("Location staging completed", false);
		}

		private void CreatePortal(Factory factory, string name, Location location, string externalId, ExternalSystem system)
		{
			var portal = factory.createPortal(name, location.LocationID, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Portal, externalId, system.Id, portal.Id);

			DisplayStatus(string.Format("Portal '{0}' added", portal.Name), false);
		}

		private void StagePortalsClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Portals Stage", true);
 
			var deerPark = Context.Locations.FirstOrDefault(x => x.LocationName == "Deer Park");

			if (deerPark == null)
			{
				DisplayStatus("**!!**  Deer Park Location missing!  **!!**", false);
				return;
			}

			var factory = new Factory(Context);

			CreatePortal(factory, "(Portal 025) Deer Park Contractor Middle Turnstile Entry", deerPark, "025", S2);
			CreatePortal(factory, "(Portal 026) Deer Park Contractor Middle Turnstile Exit", deerPark, "026", S2);
			CreatePortal(factory, "(Portal 027) Deer Park Contractor Left Turnstile Entry", deerPark, "027", S2);
			CreatePortal(factory, "(Portal 028) Deer Park Contractor Left Turnstile Exit", deerPark, "028", S2);
			CreatePortal(factory, "(Portal 029) Contractor Vehicle Gate", deerPark, "029", S2);
			CreatePortal(factory, "(Portal 030) Deer Park Contractor Right Turnstile Entry", deerPark, "030", S2);
			CreatePortal(factory, "(Portal 031) Deer Park Contractor Right Turnstile Exit", deerPark, "031", S2);

			var bayPort = Context.Locations.FirstOrDefault(x => x.LocationName == "Bayport");

			if (bayPort == null)
			{
				DisplayStatus("**!!**  Bayport Location missing!  **!!**", false);
				return;
			}

			CreatePortal(factory, "(Portal 062) Bayport North Turnstile Entry", bayPort, "062", S2);
			CreatePortal(factory, "(Portal 063) Bayport North Turnstile Exit", bayPort, "063", S2);
			CreatePortal(factory, "(Portal 064) Bayport South Turnstile Entry", bayPort, "064", S2);
			CreatePortal(factory, "(Portal 065) Bayport South Turnstile Exit", bayPort, "065", S2);
			CreatePortal(factory, "(Portal 066) Bayport Walkthrough Gate", bayPort, "066", S2);

			DisplayStatus("Portals staging completed", false);
		}

		private void CreateReader(Factory factory, string portalName, string name, string externalId, ExternalSystem system, ReaderDirection direction = ReaderDirection.Neutral)
		{
			var portal = Context.Portals.FirstOrDefault(x => x.Name == portalName);

			if (portal == null)
			{
				DisplayStatus(string.Format("**!!**  Portal ({0}) missing!  **!!**", portalName), false);
				return;
			}

			var reader = factory.createReader(name, portal.Id, direction: direction, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Reader, externalId, system.Id, reader.Id);

			DisplayStatus(string.Format("Reader '{0}' added", reader.Name), false);
		}

		private void StageReadersClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Reader Stage", true);
 
			var factory = new Factory(Context);

			CreateReader(factory, "(Portal 062) Bayport North Turnstile Entry", "062 Bayport North Turnstile Entry", "60", S2, ReaderDirection.In);
			CreateReader(factory, "(Portal 063) Bayport North Turnstile Exit", "063 Bayport North Turnstile Exit", "61", S2, ReaderDirection.Out);
			CreateReader(factory, "(Portal 064) Bayport South Turnstile Entry", "064 Bayport South Turnstile Entry", "14", S2, ReaderDirection.In);
			CreateReader(factory, "(Portal 065) Bayport South Turnstile Exit", "065 Bayport South Turnstile Exit", "59", S2, ReaderDirection.Out);
			CreateReader(factory, "(Portal 066) Bayport Walkthrough Gate", "066 Bayport Walkthrough Entry", "62", S2, ReaderDirection.In);
			CreateReader(factory, "(Portal 025) Deer Park Contractor Middle Turnstile Entry", "025 Deer Park Contractor Middle Turnstile Entry", "105", S2, ReaderDirection.In);
			CreateReader(factory, "(Portal 026) Deer Park Contractor Middle Turnstile Exit", "026 Deer Park Contractor Middle Turnstile Exit", "106", S2, ReaderDirection.Out);
			CreateReader(factory, "(Portal 027) Deer Park Contractor Left Turnstile Entry", "027 Deer Park Contractor Left Turnstile Entry", "102", S2, ReaderDirection.In);
			CreateReader(factory, "(Portal 028) Deer Park Contractor Left Turnstile Exit", "028 Deer Park Contractor Left Turnstile Exit", "103", S2, ReaderDirection.Out);
			CreateReader(factory, "(Portal 029) Contractor Vehicle Gate", "029 Contractor Vehicle Gate", "104", S2, ReaderDirection.Neutral);
			CreateReader(factory, "(Portal 030) Deer Park Contractor Right Turnstile Entry", "030 Deer Park Contractor Right Turnstile Entry", "107", S2, ReaderDirection.In);
			CreateReader(factory, "(Portal 031) Deer Park Contractor Right Turnstile Exit", "031 Deer Park Contractor Right Turnstile Exit", "108", S2, ReaderDirection.Out);

			DisplayStatus("Readers staging completed", false);
		}

		private void CreateAccessHistory(Factory factory, int groupCount)
		{
			var person = Context.Persons.RandomElement();

			var portal = Context.Portals.RandomElement();

			var reader = Context.Readers.RandomElement();

			var start = DateTime.Now;
			for (var i = 0; i < groupCount; i++)
			{
				var extId = string.Format("access{0}", i);
				var access = factory.createAccessHistory(extId, person.Id, portal.Id, reader.Id, (int)AccessType.Valid, accessed: start.Subtract(TimeSpan.FromMinutes(i)), action: EntityAction.InsertAndSubmit);
				factory.createExternalApplicationKey(EntityType.AccessLog, extId, S2.Id, access.Id, EntityAction.InsertAndSubmit);

				DisplayStatus(string.Format("Access '{0}' added", extId), false);
			}

		}

		private void StageAccessHistoryClick(object sender, EventArgs e)
		{
			DisplayStatus("Starting Access Stage", true);

			var factory = new Factory(Context);

			CreateAccessHistory(factory, 5);
			CreateAccessHistory(factory, 15);
			CreateAccessHistory(factory, 23);
			CreateAccessHistory(factory, 2);
			CreateAccessHistory(factory, 35);

			DisplayStatus("Access History staging completed", false);
		}
		#endregion

		#region Other Buttons
		private void DisplayStatus(string message, bool clearFirst)
		{
			StatusText.Text = clearFirst ? message : StatusText.Text + Environment.NewLine + message;
			StatusText.Refresh();
		}

		private void ClearButtonClick(object sender, EventArgs e)
		{
			StatusText.Text = "";
			StatusText.Refresh();
		}

		private void CloserClick(object sender, EventArgs e)
		{
			Close();
		}

		private void ExitAllButtonClick(object sender, EventArgs e)
		{
			Application.Exit();
		}
		#endregion

		private void EncryptButton_Click(object sender, EventArgs e)
		{
			StatusText.Text = string.Empty;

			if (string.IsNullOrWhiteSpace(TextToEncrypt.Text)) return;

			var password = Factory.EncryptPassword(TextToEncrypt.Text, ValidationKey);

			StatusText.Text = password;
		}

		private void EncryptToString_Click(object sender, EventArgs e)
		{
			StatusText.Text = string.Empty;

			if (string.IsNullOrWhiteSpace(TextToEncrypt.Text)) return;

			var crypt = new QuickAES();

			var password = crypt.EncryptToString(TextToEncrypt.Text);

			StatusText.Text = password;
		}

		private void DecryptButton_Click(object sender, EventArgs e)
		{
			//StatusText.Text = string.Empty;

			//if (string.IsNullOrWhiteSpace(TextToDecrypt.Text)) return;

			//var password = Factory.EncryptPassword(TextToDecrypt.Text, ValidationKey);
			var password = "Not Implemented";
			StatusText.Text = password;
		}

		private void DecryptFromString_Click(object sender, EventArgs e)
		{
			StatusText.Text = string.Empty;

			if (string.IsNullOrWhiteSpace(TextToDecrypt.Text)) return;

			var crypt = new QuickAES();

			var password = crypt.DecryptString(TextToDecrypt.Text);

			StatusText.Text = password;
		}
	}
}
