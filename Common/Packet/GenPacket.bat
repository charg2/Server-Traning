rem 표시 끄기
rem @echo off
rem PacketGenerator를 실행
START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml

rem XCOPY [ /Y 같은 이름 파일 덮어 쓰기 ]  [ "경로" ]
XCOPY /Y GenPackets.cs "../../DummyClient/Packet"
XCOPY /Y GenPackets.cs "../../Server/Packet"
XCOPY /Y ClientPacketManager.cs "../../DummyClient/Packet"
XCOPY /Y ServerPacketManager.cs "../../Server/Packet"





