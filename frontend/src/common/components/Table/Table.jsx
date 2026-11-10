import clsx from 'clsx';
import styles from './Table.module.scss';

const Table = ({ children, className = "", style }) => {
    return (
        <table className={clsx(styles.table, className)} style={style}>
            {children}
        </table>
    );
};

export default Table;
