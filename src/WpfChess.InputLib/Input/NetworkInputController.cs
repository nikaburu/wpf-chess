// using System;
// using System.ServiceModel;
// using WpfChess.ChessModel;
// using WpfChess.ChessModel.Figures;
// using WpfChess.InputLib.Exceptions;
// using WpfChess.InputLib.Services;

// namespace WpfChess.InputLib.Input
// {

//     [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
//     public sealed class NetworkInputController : InputController, IDataGet
//     {
//         #region Fields

//         private readonly string _serviceUrl; //"net.pipe://localhost"
//         private readonly string _adUrl; //"ChessService"
//         private readonly string _endpointAddress; //"net.pipe://localhost/ChessService"

//         private readonly ServiceHost _host;
//         #endregion

//         #region Constructors
//         public NetworkInputController(string serviceUrl, string endpointAddress, string adUrl, bool isStartWaiting = false)
//             : base(isStartWaiting)
//         {
//             _serviceUrl = serviceUrl;
//             _adUrl = adUrl;
//             _endpointAddress = endpointAddress;
            
//             _host = new ServiceHost(this, new Uri[] { new Uri(_serviceUrl) });
//             _host.AddServiceEndpoint(typeof(IDataGet), new NetTcpBinding(), _adUrl);

//             _host.Open();
//         }

//         #endregion

//         public override void SendInput(Cell fromCell, Cell toCell, Figure figure = null)
//         {
//             IsWaiting = true;

//             //_delegateInstance = new TurnDelegate(MakeTurn);
//             //_delegateInstance.BeginInvoke(fromCell, toCell, figure, null, null);
//             MakeTurn(fromCell, toCell, figure);
//         }

//         public override void CleanUpResources()
//         {
//             _host.Close();
//         }

//         public bool GetData(Data from, Data to)
//         {
//             MakeInput(new Cell(from.CelHorPosition, from.CellVertPosition), new Cell(to.CelHorPosition, to.CellVertPosition), to.CellFigureName);
//             return true;
//         }

//         public bool IsAvailable()
//         {
//             return true;
//         }

//         #region Private members
//         private void MakeTurn(Cell fromCell, Cell toCell, Figure figure)
//         {
//             IDataGet proxy = OpenProxy();

//             try
//             {
//                 if (!proxy.IsAvailable())
//                 {
//                     throw new NetworkException("Server not available.");
//                 }

//             }
//             catch (EndpointNotFoundException exception)
//             {
//                 throw new NetworkException("Server not available: EndpointNotFoundException", exception);
//             }

//             proxy.GetData(new Data() { CelHorPosition = fromCell.HorPosition, CellVertPosition = fromCell.VertPosition },
//                 new Data()
//                 {
//                     CelHorPosition = toCell.HorPosition,
//                     CellVertPosition = toCell.VertPosition,
//                     CellFigureName = figure != null ? figure.GetType().Name : null
//                 });
            
//         }

//         private IDataGet OpenProxy()
//         {
//             EndpointAddress endpointAddress = new EndpointAddress(new Uri(_endpointAddress), EndpointIdentity.CreateUpnIdentity("client@chessgame.wpf"));

//             ChannelFactory<IDataGet> factory = new ChannelFactory<IDataGet>
//                 (new NetTcpBinding(), endpointAddress);

//             return factory.CreateChannel();
//         }
//         #endregion
//     }
// }
