// Ligra.cpp : main project file.

#include "stdafx.h"
#include "LigraForm.h"

using namespace MetaphysicsIndustries::Ligra;

[STAThreadAttribute]
int main(array<System::String ^> ^args)
{
	// Enabling Windows XP visual effects before any controls are created
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false); 

	// Create the main window and run it
	Form^	f;
	f = gcnew LigraForm;
	Application::Run(f);
	return 0;
}
