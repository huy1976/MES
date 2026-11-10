import clsx from 'clsx';
import styles from './Table.module.scss';

const TableHeader = ({ children, className = "", style }) => {
    return (
      <th className={clsx(styles.tableHeader, className)} style={style}>
        {children}
      </th>
    );
  };
  
  export default TableHeader;
  