set WORKSPACE=..\

set GEN_CLIENT=%WORKSPACE%\Excel\Luban.Client\Luban.Client.exe
set CONF_ROOT=%WORKSPACE%\Excel\DesignerConfigs

%GEN_CLIENT% -h %LUBAN_SERVER_IP% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %WORKSPACE%\Unity\Assets\Scripts\Core\Generate\JsonConfigCode ^
 --output_data_dir %WORKSPACE%\Unity\Assets\Res\Config\JsonConfig ^
 --gen_types code_cs_unity_bin,data_bin ^
 -s all 

pause