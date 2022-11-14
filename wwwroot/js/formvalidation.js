import * as base from './basevalidation.js';

form.addEventListener('submit', e =>
{
	const descriptionValue = document.getElementById('descriptionInput').value.trim();
	const dateValue = Date.parse(document.getElementById('dateInput').value);
	const todayValue = Date.parse(new Date().toJSON().slice(0, 10).replace(/-/g, '-'));
	const gap = 32400000000;

	var dateMessage = '';
	var descriptionMessage = '';

	if (isNaN(dateValue))
	{
		dateMessage += 'nebol zadaný dátum';
	}
	else
	{
		if (todayValue > dateValue)
		{
			dateMessage += 'dátum musí byť budúci';
		}	
	}

	if (descriptionValue !== '')
	{
		if (base.injectionProtection(descriptionValue))
		{
			descriptionMessage += 'poznámka obsahuje nepovolené kľúčové slová (select alter update delete)';
		}
	}

	base.unsetFor('description');
	base.unsetFor('date');

	if (descriptionMessage === '')
	{
		base.setSuccessFor('description');
	}
	else
	{
		base.setErrorFor('description', descriptionMessage);
	}

	if (dateMessage === '')
	{
		base.setSuccessFor('date');
	}
	else
	{
		base.setErrorFor('date', dateMessage);
	}

	var submit;

	if (descriptionMessage === '' && dateMessage === '')
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