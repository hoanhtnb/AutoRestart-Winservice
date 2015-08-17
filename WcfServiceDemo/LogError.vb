<Serializable()> _
Public NotInheritable Class LogError
    Public Shared log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
End Class
