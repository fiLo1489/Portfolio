form.addEventListener('submit', e =>
{
	const descriptionValue = document.getElementById('descriptionInput').value.trim();
	const dateValue = Date.parse(document.getElementById('dateInput').value);
	const todayValue = Date.parse(new Date().toJSON().slice(0, 10).replace(/-/g, '-'));
	const gap = 32400000000;

	var dateMessage = '';
	var descriptionMessage = '';

	if (!isNaN(dateValue))
	{
		if (todayValue > dateValue)
		{
			dateMessage += 'dátum musí byť budúci';
		}
	}

	if (descriptionValue !== '')
	{
		if (injectionProtection(descriptionValue))
		{
			descriptionMessage += 'poznámka obsahuje nepovolené kľúčové slová (select alter update delete)';
		}
	}

	unsetFor('description');
	unsetFor('date');

	if (descriptionMessage === '')
	{
		setSuccessFor('description');
	}
	else
	{
		setErrorFor('description', descriptionMessage);
	}

	if (dateMessage === '')
	{
		setSuccessFor('date');
	}
	else
	{
		setErrorFor('date', dateMessage);
	}

	var submit;

	if (descriptionMessage === '' && dateMessage === '') {
		submit = true;
	}
	else {
		submit = false;
	}

	if (!submit) {
		e.preventDefault();
	}
});

function setErrorFor(element, message)
{
	const parent = document.getElementById(element);
	const children = document.getElementById(element + 'Message');

	parent.classList.add('error');
	children.textContent = message;
	children.style.visibility = 'visible';
}

function setSuccessFor(element)
{
	const parent = document.getElementById(element);

	parent.classList.add('success');
}

function unsetFor(element)
{
	const parent = document.getElementById(element);
	const children = document.getElementById(element + 'Message');

	if (parent.classList.contains('error'))
	{
		parent.classList.remove('error');
		children.textContent = '';
		children.style.visibility = 'hidden';
	}

	if (parent.classList.contains('success'))
	{
		parent.classList.remove('success');
		children.textContent = '';
		children.style.visibility = 'hidden';
	}
}

function injectionProtection(value)
{
	if (value.includes('insert') || value.includes('select') || value.includes('update') || value.includes('delete'))
	{
		return true;
	}
	else
	{
		return false;
	}
}