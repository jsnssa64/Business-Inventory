

export default function formatString(template: string, ...values: (string | number)[]) {
    return template.replace(/{(\d+)}/g, (_, index) => {
        const i = parseInt(index, 10);
        return values[i]?.toString() ?? `{${index}}`;
    });
  }