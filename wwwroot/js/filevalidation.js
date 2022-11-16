import * as base from './basevalidation.js';

window.URL = window.URL || window.webkitURL;

form.addEventListener('submit', e =>
{
	const fileElement = document.getElementById('fileInput');

	var fileMessage = '';

	if (fileElement.files.length == 0)
	{
		fileMessage += 'nebol zvolený žiaden súbor';
	}
	else
	{
		const file = fileElement.files[0];

		if (file['type'] !== 'image/jpeg') {
			fileMessage += 'súbor nie je vo formáte .jpg';
		}
		else
		{
			var resolution = base.isResolutionValid(file);

			if (!resolution)
			{
				fileMessage += 'fotografia nemá požadovaný pomer 16:10 alebo 10:16';
			}
		}
	}

	base.unsetFor('file');

	if (fileMessage === '')
	{
		base.setSuccessFor('file');
	}
	else {
		base.setErrorFor('file', fileMessage);
	}

	if (fileMessage !== '')
	{
		e.preventDefault();
	}
});