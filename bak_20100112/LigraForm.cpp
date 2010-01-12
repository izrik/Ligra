#include "StdAfx.h"
#include "LigraForm.h"

NAMESPACE_START;


using namespace System;
interface class IMyIfc{};
public ref class MyClass: public IMyIfc{};

public ref class MyDerivedClass: public MyClass{};

System::Void LigraForm::LigraForm_Load(System::Object^  sender, System::EventArgs^  e)
{
	Type^ imyifcType = IMyIfc::typeid;
	MyClass^ mc = gcnew MyClass;
	Type^ mcType = mc->GetType();
	MyClass^ mdc = gcnew MyDerivedClass;
	Type^ mdcType = mdc->GetType();
	array<Int32>^arr = gcnew array<Int32>(10);
	Type^ arrayType = Array::typeid;

	this->OutputTextbox->Text +=
		String::Format( "Is Int32[] an instance of the Array class? {0}.\r\n", arrayType->IsInstanceOfType( arr ) ) + 
		String::Format( "Is myclass an instance of MyClass? {0}.\r\n", mcType->IsInstanceOfType( mc ) ) + 
		String::Format( "Is myderivedclass an instance of MyClass? {0}.\r\n", mcType->IsInstanceOfType( mdc ) ) + 
		String::Format( "Is myclass an instance of IMyIfc? {0}.\r\n", imyifcType->IsInstanceOfType( mc ) ) + 
		String::Format( "Is myderivedclass an instance of IMyIfc? {0}.\r\n", imyifcType->IsInstanceOfType( mdc ) );

	Type^	t1;
	Type^	t2;

	t1 = Expression::typeid;
	t2 = Literal::typeid;

	this->OutputTextbox->Text = 
		String::Format( "Is t1 assignable from t2 ? {0}. \r\n", t1->IsAssignableFrom(t2));


	this->OutputTextbox->Text += "\r\n";

	this->evalcount = 0;
	
	this->EvalButton->Click += gcnew EventHandler(this, &LigraForm::EvalButton_Click);
}

void LigraForm::EvalButton_Click(Object^, EventArgs^)
{
	SolusEngine^	se;
	Expression^		e;

	this->evalcount++;
	se = gcnew SolusEngine;
	e = se->Parse(this->InputTextBox->Text);
	this->OutputTextbox->Text += this->evalcount.ToString() + ": " + se->Eval(e)->Value.ToString() + "\r\n";
}



NAMESPACE_END