using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
namespace NTier.Validations
{

    public interface iValidate
    {
        clsMsg validate(clsCmd cmd);
    }


    internal interface iValidations : IList<iValidate>, iValidate
    {
        clsMsg validate(clsCmd cmd);
    }

    internal abstract class validateBase : iValidate
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }

        public abstract clsMsg validate(clsCmd cmd);
    }

   


    internal class clsValidation : List<validateBase>, iValidate
    {

        public void addTextField(string sField, string sFieldTitle, int iMaxLength = 0)
        {
            var f = new validateText() { FieldName = sField, FieldTitle = sFieldTitle, MaxLength = iMaxLength, Required = true };
            this.Add(f);
            
        }


        public void addNumberField(string sField
            , string sFieldTitle
            , bool required = false
            , bool allowZero = true)
        {

            var f = new validateNumber() { FieldName = sField, FieldTitle = sFieldTitle, Required = required, AllowZero = allowZero };
            this.Add(f);
            
        }

        public void addCheckConstraint(string sField, string sFieldTitle, string sCommaSepratedValues)
        {
            var f = new validateCheckConstraint(sCommaSepratedValues.Split(','));
            f.FieldName = sField;
            f.FieldTitle = sFieldTitle;
            this.Add(f);
        }

        public void addEmailField(string sField, string sFieldTitle, bool required)
        {
            var f = new validateEmail() { FieldName = sField, FieldTitle = sFieldTitle, Required = required };
            this.Add(f);
            
        }

        public void addDateField(string sField, string sFieldTitle, string sFormat, bool required)
        {
            var f = new validateDate() { FieldName = sField, FieldTitle = sFieldTitle, Required = required, DateFormat = sFormat };
            this.Add(f);
            
        }

        internal void addDropDownField(string sField, string sFieldTitle)
        {
            var f = new validationDropDownID() { FieldName = sField, FieldTitle = sFieldTitle };
            this.Add(f);
        }

        public void addDuplicate(adapter.iAdapter adapter
            , string sTableName
            , string sPrimaryKeyField
            , string sField
            , string sFieldTitle
            , bool required)
        {
            var f = new validateDuplicate(adapter,sTableName,sPrimaryKeyField,sField) { FieldName = sField, FieldTitle = sFieldTitle, Required = required};
            this.Add(f);
            
        }

        

        public clsMsg validate(clsCmd cmd)
        {

            foreach (var f in this)
            {

                var msg = f.validate(cmd);
                if (msg.Validated == false) return msg;
            }

            return g.msg();
        }






        

    }

}

