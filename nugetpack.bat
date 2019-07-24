
cd Cflashsoft.Framework.Types
nuget pack -prop Configuration=Release

cd ..

cd Cflashsoft.Framework.AppServices
nuget pack -prop Configuration=Release

cd ..

cd Cflashsoft.Framework.EntityAppServices
nuget pack -prop Configuration=Release

cd ..

cd Cflashsoft.Framework.EntityCoreAppServices
nuget pack -prop Configuration=Release

cd ..

cd Cflashsoft.Framework.WcfAppServices
nuget pack -prop Configuration=Release

cd ..
