set WORKSPACE=..\

set GEN_CLIENT_SERVER=%WORKSPACE%\Excel\Luban.ClientServer\Luban.ClientServer.dll
set CONF_ROOT=%WORKSPACE%\Excel\DesignerConfigs

dotnet %GEN_CLIENT_SERVER% -j cfg -- ^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %WORKSPACE%\Unity\Assets\Scripts\Core\Generate\JsonConfigCode ^
 --output_data_dir %WORKSPACE%\Unity\Assets\Res\Config\JsonConfig ^
 --gen_types code_cs_unity_json,data_json ^
 -s all

@REM pause