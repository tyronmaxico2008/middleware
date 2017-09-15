using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Validations
{


    public interface iValidate
    {
        clsMsg validate(clsCmd cmd);
    }

    public interface iValidations : IList<iValidate>, iValidate
    {
        clsMsg validate(clsCmd cmd);
    }


    public abstract class validateBase : iValidate
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }

        public abstract clsMsg validate(clsCmd cmd);
    }

    public class validateText : validateBase
    {
        public int MaxLength { get; set; }
        public bool Required { get; set; }

        public override clsMsg validate(clsCmd cmd)
        {
            string sVal = cmd.getStringValue(this.FieldName);

            if (Required)
                if (sVal.isEmpty())
                    return g.msg(string.Format("Field value [{0}] is empty !", this.FieldTitle));


            if (MaxLength > 0 && sVal.isEmpty() == false && sVal.Length > MaxLength)
                return g.msg(string.Format("Max Legnth of field [{0}] is [{1}], you have entered more than that length !", this.FieldTitle, this.MaxLength));

            return g.msg("");
        }

    }



    public class validateCheckConstraint : validateBase
    {
        string[] values;

        public validateCheckConstraint(string[] Values)
        {
            values = Values;
        }
        public override clsMsg validate(clsCmd cmd)
        {
            string sVal = cmd.getStringValue(FieldName);
            if (!sVal.Contains(values))
                return g.msg("Wrong value entered in [" + this.FieldTitle + "] ! ");

            return g.msg("");
        }

    }


    public class validateUnknown : validateBase
    {

        Func<clsCmd, clsMsg> _fnValidate = null;

        public validateUnknown(Func<clsCmd, clsMsg> fnValidate)
        {
            _fnValidate = fnValidate;
        }

        public override clsMsg validate(clsCmd cmd)
        {
            if (_fnValidate != null)
                return _fnValidate(cmd);
            return g.msg("");
        }

    }

    public class validateNumber : validateText
    {


        public bool AllowZero { get; set; }
        public override clsMsg validate(clsCmd cmd)
        {

            string sVal = cmd.getStringValue(this.FieldName);

            if (cmd.ContainFields(this.FieldName) && sVal.isEmpty())
            {
                cmd.setValue(this.FieldName, 0);
                return g.msg("");
            }

            if (sVal.isEmpty() == false && g.isNumeric(sVal) == false)
            {
                return g.msg(string.Format("Wrong numeric value [{1}] \nYou have not specified proper numeric value for field [{0}]", this.FieldTitle, sVal));

            }

            if (AllowZero == false
                && g.parseDouble(sVal) == 0.0)
                return g.msg(string.Format("The field [{0}] doesn't allow zero value, please specify some value for this field !", this.FieldTitle));


            return g.msg();
        }
    }


    public class validateEmail : validateText
    {
        public override clsMsg validate(clsCmd cmd)
        {

            string sVal = cmd.getStringValue(this.FieldName);

            if (sVal.isEmpty() == false && g.isEmail(sVal) == false)
                return g.msg(string.Format("Wrong Email value [{1}] \nYou have not specified proper email value for field [{0}]", this.FieldTitle, sVal));
            return g.msg("");
        }
    }
    public class validateDate : validateText
    {

        public string DateFormat { get; set; }

        public override clsMsg validate(clsCmd cmd)
        {

            var msg = base.validate(cmd);
            if (msg.Validated == false) return msg;

            string sVal = cmd.getStringValue(this.FieldName);

            

            if (sVal.isEmpty() == false && g.isDate(sVal, this.DateFormat) == false)
            {
                return g.msg(string.Format("Wrong Date value [{1}] \nYou have not specified proper Date value for field [{0}]", this.FieldTitle, sVal));
            }
            else
                if (g.isDate(sVal, this.DateFormat))
                {
                    cmd.setValue(this.FieldName, cmd.getDate(this.FieldName,this.DateFormat).ToString("dd/MMM/yyyy"));
                }

            return g.msg("");
        }

    }

    public class clsValidation : List<validateBase>, iValidate
    {

        public validateText addTextField(string sField, string sFieldTitle, int iMaxLength = 0)
        {
            var f = new validateText() { FieldName = sField, FieldTitle = sFieldTitle, MaxLength = iMaxLength, Required = true };
            this.Add(f);
            return f;
        }


        public validateNumber addNumberField(string sField
            , string sFieldTitle
            , bool required = false
            , bool allowZero = true)
        {

            var f = new validateNumber() { FieldName = sField, FieldTitle = sFieldTitle, Required = required, AllowZero = allowZero };
            this.Add(f);
            return f;
        }


        public validateCheckConstraint addCheckConstraint(string sField, string sFieldTitle, string sCommaSepratedValues)
        {


            var f = new validateCheckConstraint(sCommaSepratedValues.Split(','));
            f.FieldName = sField;
            f.FieldTitle = sFieldTitle;



            this.Add(f);
            return f;
        }


        public validateEmail addEmailField(string sField, string sFieldTitle, bool required)
        {
            var f = new validateEmail() { FieldName = sField, FieldTitle = sFieldTitle, Required = required };
            this.Add(f);
            return f;
        }

        public validateDate addDateField(string sField, string sFieldTitle, string sFormat, bool required)
        {
            var f = new validateDate() { FieldName = sField, FieldTitle = sFieldTitle, Required = required, DateFormat = sFormat };
            this.Add(f);
            return f;
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

