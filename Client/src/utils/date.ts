export const formatLocalDateTime = (value?: string | Date) => {
    if (!value) return '';
    const date = new Date(value);
    return date.toLocaleString('ru-RU', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
};

