set WORKSPACE=..\..\
set GEN_CLIENT=%WORKSPACE%\Excel\Luban.Client\Luban.Client.exe

set CONF_ROOT=%WORKSPACE%\Excel\DesignerConfigs

%GEN_CLIENT% -h %LUBAN_SERVER_IP% -j cfg --generateonly --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir %WORKSPACE%\Unity\Assets\Res\Config\JsonConfig ^
 --gen_types data_json ^
 -s all
pause