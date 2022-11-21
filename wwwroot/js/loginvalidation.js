import * as base from './basevalidation.js';

form.addEventListener('submit', e =>
{
	const mailValue = document.getElementById('mailInput').value.trim();
	const passwordValue = document.getElementById('passwordInput').value.trim();

	var mailMessage = '';
	var passwordMessage = '';

	if (mailValue === '')
	{
		mailMessage += 'nebol zadaný mail, ';
	}
	else
	{
		if (base.injectionProtection(mailValue))
		{
			mailMessage += 'zadaný mail obashuje nepovolené kľúčové slová (select alter update delete), ';
		}

		if (!base.isMailValid(mailValue))
		{
			mailMessage += 'zadaný mail má nesprávny tvar, ';
		}
	}

	if (passwordValue === '')
	{
		passwordMessage += 'nebolo zadané heslo, ';
	}
	else
	{
		if (base.injectionProtection(passwordValue)) {
			passwordMessage += 'zadané heslo obashuje nepovolené kľúčové slová (select alter update delete), ';
		}

		if (!base.isPasswordValid(passwordValue))
		{
			passwordMessage += 'heslo musí obsahovať veľký a malý znak, číslo a bezpečnostný znak (! . & # /), ';
		}
	}

	base.unsetFor('mail');
	base.unsetFor('password');

	if (mailMessage === '')
	{
		base.setSuccessFor('mail');
	}
	else {
		base.setErrorFor('mail', mailMessage.substring(0, mailMessage.length - 2));
	}

	if (passwordMessage === '')
	{
		base.setSuccessFor('password');
	}
	else {
		base.setErrorFor('password', passwordMessage.substring(0, passwordMessage.length - 2));
	}

	var submit;

	if (passwordMessage === '' && mailMessage === '')
	{
		submit = true;
	}
	else
	{
		submit = false;
	}

	if (!submit)
	{
		e.preventDefault();
	}
});