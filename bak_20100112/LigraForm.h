#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace MetaphysicsIndustries { namespace Ligra {

	/// <summary>
	/// Summary for LigraForm
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class LigraForm : public System::Windows::Forms::Form
	{
	public:
		LigraForm(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~LigraForm()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::TextBox^  OutputTextbox;
	private: System::Windows::Forms::TextBox^  InputTextBox;
	private: System::Windows::Forms::Button^  EvalButton;
	protected: 



	protected: 

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->OutputTextbox = (gcnew System::Windows::Forms::TextBox());
			this->InputTextBox = (gcnew System::Windows::Forms::TextBox());
			this->EvalButton = (gcnew System::Windows::Forms::Button());
			this->SuspendLayout();
			// 
			// OutputTextbox
			// 
			this->OutputTextbox->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->OutputTextbox->Location = System::Drawing::Point(12, 12);
			this->OutputTextbox->Multiline = true;
			this->OutputTextbox->Name = L"OutputTextbox";
			this->OutputTextbox->ReadOnly = true;
			this->OutputTextbox->Size = System::Drawing::Size(368, 320);
			this->OutputTextbox->TabIndex = 0;
			// 
			// InputTextBox
			// 
			this->InputTextBox->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->InputTextBox->Location = System::Drawing::Point(12, 341);
			this->InputTextBox->Name = L"InputTextBox";
			this->InputTextBox->Size = System::Drawing::Size(287, 20);
			this->InputTextBox->TabIndex = 1;
			// 
			// EvalButton
			// 
			this->EvalButton->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->EvalButton->Location = System::Drawing::Point(305, 338);
			this->EvalButton->Name = L"EvalButton";
			this->EvalButton->Size = System::Drawing::Size(75, 23);
			this->EvalButton->TabIndex = 2;
			this->EvalButton->Text = L"Eval";
			this->EvalButton->UseVisualStyleBackColor = true;
			// 
			// LigraForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(392, 373);
			this->Controls->Add(this->EvalButton);
			this->Controls->Add(this->InputTextBox);
			this->Controls->Add(this->OutputTextbox);
			this->Name = L"LigraForm";
			this->Text = L"LigraForm";
			this->Load += gcnew System::EventHandler(this, &LigraForm::LigraForm_Load);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

	protected:

		int	evalcount;

	private:

		void EvalButton_Click(Object^, EventArgs^);

		System::Void LigraForm_Load(System::Object^  sender, System::EventArgs^  e);

	};

} }
