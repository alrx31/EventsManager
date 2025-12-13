const parseAsUtcIfNeeded = (value: string | Date): Date | null => {
    try {
        if (value instanceof Date) return value;
        const hasTz = /([zZ])|([+-]\d{2}:?\d{2})$/.test(value);
        const normalized = hasTz ? value : `${value}Z`;
        const d = new Date(normalized);
        return isNaN(d.getTime()) ? null : d;
    } catch {
        return null;
    }
};

export const formatLocalDateTime = (value?: string | Date) => {
    if (!value) return '';
    const date = parseAsUtcIfNeeded(value);
    if (!date) return '';
    return date.toLocaleString('ru-RU', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
};

