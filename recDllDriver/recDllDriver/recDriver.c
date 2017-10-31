#include <Windows.h>
#include "RESOURCE.H"
#include "Export.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PSTR szCmdLine, int iCmdShow)
{
	StartDialog();
	/*if (-1 == DialogBox(hInstance, TEXT("Record"), NULL, DlgProc))
	{
		MessageBox(NULL, TEXT("This program requires Windows NT!"),
			"error", MB_ICONERROR);
	}*/
	return 0;
}